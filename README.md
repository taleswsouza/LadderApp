# LadderApp

## A brief history and context

This project was finalized at the end of 2010 as my undergraduate thesis in Information Systems. It reflects some of my areas of interest (development, object-oriented programming, industrial automation, microcontrollers).

At the time, I would have liked to have developed this project in Java Web, but the rush of life, work, and college did not allow me to believe it was a viable work.

Almost ten years have passed since I finished this project. So, last year, I put it on GitHub but, I still had many ideas for it and still had done nothing about these ideas.

## The project

The project LadderApp allows you to develop a program in ladder language (standard IEC 61131-3) and send the executable to the microcontroller read a previously uploaded file from the microcontroller and "remount" the ladder.

The "base" microcontroller code inside the project was designed only for [MSP430](http://www.ti.com/microcontrollers/msp430-ultra-low-power-mcus/overview.html) microcontroller from Texas Instruments.

The tool has three main functions:
- Develop a ladder program in the GUI and write that program to the microcontroller to be executed
- Read a previously written program from the microcontroller and "remount" the ladder at the GUI.
- Simulate the PLC/microcontroller work at the tool without any extra hardware.

### Writing to a microcontroller

To this work, the tool will perform the conversion of a ladder program developed at GUI into a C language program to the final microcontroller, allowing this program to be compiled and sent to the microcontroller. 

With an opened ladder program at the GUI of the tool, ready to work, when you click on the menu "Microcontroller/Communication/Upload Program", in the background, the tool will convert that ladder program to a C language program with the whole base code for the PLC work linked with ladder logic developed in the tool. After that, also in the background, it will compile the generated C program (using [MSPGCC](https://www.ti.com/tool/MSP430-GCC-OPENSOURCE) - you need to install first) and then it will send executable (the compilation result) to the microcontroller connected on USB (using MSPJTAG USB tool).

### Reading from a microcontroller

When you write a ladder program to the microcontroller, an option could be enabled (default option) to save the ladder logic inside the executable written to the microcontroller. So, when you read a program from a microcontroller in which that option was enabled, the tool could "remount" the ladder at the GUI from that executable program read.

### Simulating a microcontroller/plc work

The tool allows the user to simulate the execution of the ladder code directly on the GUI interface and interact with the execution toggle bit and toggle bit pulse options.

### Future ideas or Roadmap

I have many ideas for this project, and some of them are:
- English translation
- Make this project a multiplatform tool. My first challenge is to adapt it to [Arduino](https://www.arduino.cc/). In the future, we will adapt it to others (PIC, STM32, ...)
- Convert the project to a web SAAS 
- Internationalization

![LadderApp screen with an open program](images/ladderapp-running.gif)