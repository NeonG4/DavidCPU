# David CPU

## About

This is an assembler for a [CPU emulator](https://scratch.mit.edu/projects/1059169040/) made in Scratch. It translates between the opcodes and the binary, which the CPU can read. 

## Assembly 

### Functions

| name   | params| description       |
| :--- | :--- | :--- |
| `add` | `reg1 reg2 --> reg3` | Adds register `reg1` with register `reg2` and stores in register `reg3` |
| `add` | `reg1 const --> reg3` | Adds register `reg1` with number `constant` and stores in register `reg3` |
| `sub` | `reg1 reg2 --> reg3` | Subtracts register `reg1` with register `reg2` and stores in register `reg3` |
| `mul` | `reg1 reg2 --> reg3` | Multiplies register `reg1` with register `reg2` and stores in register `reg3` |
| `div` | `reg1 reg2 --> reg3` | Divides register `reg1` with register `reg2` and stores in register `reg3`, remainder stored in register `r1` |
| `and` | `reg1 reg2 --> reg3` | Bitwise AND compares `reg1` with `reg2` and stores in `reg3` |
| `or` | `reg1 reg2 --> reg3` | Bitwise OR compares `reg1` with `reg2` and stores in `reg3` |
| `not` | `reg1 --> reg2` | Bitwise NOT compares `reg1` stores in `reg2` |
| `nand` | `reg1 reg2 --> reg3` | Bitwise NAND compares `reg1` with `reg2` and stores in `reg3` |
| `xor` | `reg1 reg2 --> reg3` | Bitwise XOR compares `reg1` with `reg2` and stores in `reg3` |
| `exit` | none | Ends the program |
| `jmp` | `reg1` | Absolute jump by setting the instruction pointer to `reg1` value |
| `jmp` | `const` | Relative jump by changing the instruction pointer by `const` instruction, `const` can be number or label |
| `jmp.if` | `const reg1` | Relative jump by changing the instruction pointer by `const` instruction if `reg1` is true, passing if false, `const` can be number or label |
| `val` | `const reg1` | loads `const` into register `reg1`  |
| `mov` | `[const] --> reg1` | loads `[const]` into register `reg1` |
| `mov` | `[reg2] --> reg1` | loads `[reg2]` into register `reg1` | 
| `mov` | `reg1 --> [const]` | loads register value `reg1` into `[const]` in memory | 
| `mov` | `reg1 --> [reg2]` | loads register value `reg1` into `[reg2]` in memory | 
| label identifier | none | used in jumps, written as `label:` where "label" is the name | 
| `call` | `label` | used for functions, `label` will have braces holding the code it contains | 

### Memory Mapped IO

There is an MMIO from memory address __1000__ to memory address __1594__. Each character is two bytes. Encoding is based on 

"__abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\`~!@#$%^&*()-_=+[]{}|\\;':",./<>?__"

 For example, __a__ is encoded as `1`, wereas __A__ is encoded as `27`. The console is 27x11 characters. 

*Be aware! Each character takes two bytes in memory.*

Note: Space is encoded as the value `0`. The console is filled with the space character in default. 