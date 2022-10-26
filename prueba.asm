Archivo: prueba.cpp
Fecha: 26/10/2022 09:31:51 a. m.
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
Mov AX, 1
Push AX
Mov AX, 1
Push AX
Pop AX
Pop BX
JNE
Mov AX, 2
Push AX
Mov AX, 2
Push AX
Pop AX
Pop BX
JNE
Mov AX, 2
Push AX
Pop AX
MOV y,AX
Mov AX, 3
Push AX
Mov AX, 3
Push AX
Pop AX
Pop BX
JNE
Mov AX, 3
Push AX
Pop AX
MOV a,AX
if3:
if2:
Mov AX, 1
Push AX
Pop AX
MOV x,AX
if1:
RET
END
