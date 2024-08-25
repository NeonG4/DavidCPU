using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Metadata;
using System.Text;

namespace DavidAsmCore
{
    public class Writer
    {
        // private readonly TextWriter _ouput;

        private List<byte> _bytes = new List<byte>();

        // public int CurrentByteOffset { get; private set; }
        private Dictionary<int, StringBuilder> _annotations = new Dictionary<int, StringBuilder>();

        public Writer(TextWriter logger)
        {
        }

        // Get the list of bytes we emitted. 
        public void WriteToFile(TextWriter output)
        {
            ApplyTouchups();


            // foreach(var b in _bytes)
            for(var i = 0; i < _bytes.Count; i++)
            {
                var b = _bytes[i];

                if (_annotations.TryGetValue(i, out var sb))
                {
                    output.Write(sb.ToString());
                }

                string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');

                // Emit byte as binary. 
                output.WriteLine(binaryString);
            }
        }

        private void WriteAnnotation(string line)
        {
            var offset = this._bytes.Count;

            if (!_annotations.TryGetValue(offset, out var sb))
            {
                sb = new StringBuilder();
                _annotations.Add(offset, sb);
            }
            sb.AppendLine(line);
        }

        public void WriteComment(string comment)
        {
            this.WriteAnnotation($"// {comment}");
        }

        public void WriteBlankLine()
        {
            this.WriteAnnotation("");
        }

        public void WritePaddingByte()
        {
            this.WriteByte(0);
        }

        public void WriteOp(Opcode opcode)
        {
            this.WriteByte((byte) opcode);
        }

        public void WriteReg(Register reg)
        {
            this.WriteByte((byte)reg.Value);
        }

        public void WriteI16(Int16 num)
        {
            this.WriteByte((byte)(num >> 8));
            this.WriteByte((byte)(num & 0xFF));
        }

        public void WriteByte(byte b)
        {
            // string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');

            // Emit byte as binary. 
            //_ouput.WriteLine(binaryString);
            _bytes.Add(b);

            // this.CurrentByteOffset++;
        }


        public void MarkLabel(Label label)
        {
            if (_labelOffsets.ContainsKey(label))
            {
                throw new InvalidOperationException($"Label '{label}' is already defined.");
            }

            _labelOffsets[label] = this._bytes.Count;
        }

        // WriteLabel, will add to touchup. 
        // Where is each label used? 
        public void WriteLabel(Label label, TouchupKind kind)
        {
            var offset = this._bytes.Count;
                        
            if (!_touchUps.TryGetValue(label, out var list))
            {
                list = new List<int>();
                _touchUps.Add(label, list);
            }

            // $$$ include kind
            list.Add(offset);

            // Placeholder to fill in later. 
            this.WriteI16(0);
        }

        // Map of label to where it's used. 
        private readonly Dictionary<Label, List<int>> _touchUps = new Dictionary<Label, List<int>>();
        
        // Track map of labels to their offets. 
        private readonly Dictionary<Label, int> _labelOffsets = new Dictionary<Label, int>();

        public void ApplyTouchups()
        {
            foreach(var kv in _touchUps)
            {
                Label l = kv.Key;
                var offsets = kv.Value;

                if (!_labelOffsets.TryGetValue(l, out var dest))
                {
                    throw new InvalidOperationException($"Label '{l}' is not defined");
                }

                foreach(var offset in offsets)
                {
                    // $$$ use kind.
                    // Relative? Specify number of 4-byte instructions. 
                    var startInstruction = offset - 1;
                    var delta = (dest - startInstruction) / 4;
                    this.Touchup(offset, delta);
                }
            }
        }

        private void Touchup(int address, int newValue)
        {
            var num = (Int16)newValue;

            _bytes[address] = (byte)(num >> 8);
            _bytes[address+1] = (byte)(num & 0xFF);
        }
    }

    public enum TouchupKind
    {
        Relative,
        Absolute
    }

    /*
    public class LabelInfo
    {
        // Where a label is used.
        public List<int> _used = new List<int>();

        // Value the label resolves to. 
        public int _offset;

        // source information, like where it's defined. 
    }*/
}
