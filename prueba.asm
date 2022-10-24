Archivo: prueba.cpp
Fecha: 24/10/2022 09:48:19 a. m.
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
Mov AX, 61
Push AX
Pop AX
Pop BX
Mov AX, 0
Push AX
Pop AX
Mov AX, 5
Push AX
Pop AX
Pop BX
Mov AX, 0
Push AX
Pop AX
if2:
Mov AX, 10
Push AX
Pop AX
if1:
RET
END
