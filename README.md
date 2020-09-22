# LadderApp

## A little of history and context

This project was finalized at the end of 2010 as my undergraduate thesis in the Information Systems. It reflects some of my areas of interest (development, object-oriented programming, industrial automation, microcontrollers).

At the time, I would have liked to have developed this project in Java Web, but the rush of life, work and college did not allowed me to believe it was a viable work.

Almost ten years have passed since I finished this project. So, last year, I put it on github, but, I still had many ideas for it and still had done nothing about these ideas.

## The project

The project LadderApp basically allows you to develop an program in ladder language (standard IEC 61131-3) and send the executable to microcontroler read a previous uploaded file from  the microcontroller and "remount" the ladder.

The "base" microcontroller code inside the project was designed only for [MSP430](http://www.ti.com/microcontrollers/msp430-ultra-low-power-mcus/overview.html) microcontroller  from Texas Instruments.

The tool has three main functions:
- Develop a ladder program in the GUI and write that programa to the microcontroller to be executed
- Read a previous writed program from microcontroller and "remount" the ladder at the GUI.
- Simulate the PLC/microcontroller work at the tool without any extra hardware.

### Writing to microcontroller

To do this work, the tool will perform the conversion of an ladder program developed at GUI into an C language program to final microcontroller, allowing this program to be compiled and sended to the microcontroller. 

With an opened ladder program at the GUI of the tool, ready to work, when you click on the menu "Microcontroller/Communication/Upload Program", in background, the tool will convert that ladder program to a C language program with the whole base code for the PLC work linked with ladder logic developed in the tool. After that, also in background, it will compile the generated C program (using [MSPGCC](https://www.ti.com/tool/MSP430-GCC-OPENSOURCE) - you need to install first) and then it will send executable (compilation result) to the microcontroller connected on USB (using MSPJTAG usb tool).

### Reading from microcontroller

When you write a ladder program to the microcontroller, an option could be enable (default option) to save the ladder logic inside the executable writed to the microcontroller. So, when you read a program from a microcontroller in which that option was enabled, the tool could "remount" the ladder at the GUI from that executable program readed.

### Simulating a microcontroller/plc work

The tool allows the user to simulate the execution of the ladder code directly on the GUI interface and interact with the execution toggle bit and toggle bit pulse options.

### Future ideas or roadmap

I has many ideas for this project, and some of them are:
- English tranlation
- Make this project a multiplatform tool. My first challenge is to adapt it to [Arduíno](https://www.arduino.cc/), in the future we will adapt it to others (PIC, STM32, ...)
- Convert the project to a web SAAS 
- Intenacionalization

![LadderApp screen with an open program] (images / tela-ladderapp.PNG)