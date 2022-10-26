Archivo: prueba.cpp
Fecha: 25/10/2022 09:55:35 a. m.
make COM
include 'emu 8086.inc'
ORG 100h
Variables: 
	area DW ? 
	radio DW ? 
	pi DW ? 
	resultado DW ? 
	a DW ? 
	d DW ? 
	altura DW ? 
	x DW ? 
	y DW ? 
	i DW ? 
	j DW ? 
Mov AX, 61
Push AX
Pop AX
MOV y,AX
Mov AX, 61
Push AX
Pop AX
Pop BX
JNE
Mov AX, 10
Push AX
Pop AX
MOV x,AX
if1:
RET
END
