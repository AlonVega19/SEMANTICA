Archivo: prueba.cpp
Fecha: 21/10/2022 09:20:29 a. m.
make COM
include 'emu 8086.inc'
ORG 100h
Mov AX, 2
Push AX
Pop AX
Pop BX
Pop AX
Mov AX, 0
Push AX
Pop AX
Pop BX
Mov AX, 0
Push AX
Pop AX
Pop AX
Pop BX
SUB AX, BX
Push AX
Pop AX
Pop BX
