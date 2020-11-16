using System;
using System.Security.Cryptography;

namespace Lab_PAOIiAS_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //регистры
            int EAX = 0, EBX = 0, ECX = 0, EDX = 0;
            //flag
            int CF = 0;
            //value
            int value;
            // счетчик команд, команд операций
            int PC, OpCode;

            //массив чисел для сложения
            //int[] numbers = new int[] { 5, 3, 1, 6, 10 };
            int[] a = new int[5] { 9456431, 9456434, 532, 44, 9456434 };
            int[] b = new int[5] { 4531, 4531, 822, 19, 51022 };
            long expectedResult = 0;

            for (int i = 0; i < a.Length; i++)
            {
                expectedResult = expectedResult + a[i]* b[i];
            }
            Console.WriteLine("Expected result : {0}", expectedResult);
            //оперативная память
            int[] cmem = new int[30];
            cmem[0] = 0x10000014;//load a1 num to EAX 
            cmem[1] = 0x11000019;//load b1 num to EBX
            cmem[2] = 0x30000002;//MUL EBX

            cmem[3] = 0x11000015;//load a2 num to EBX
            cmem[4] = 0x1200001A;//load b2 num to ECX
            cmem[5] = 0x20000003;//MUL ECX

            cmem[7] = 0x40000000;//add EAX, EBX 

            cmem[8] = 0x11000016;//load a3 num to EBX
            cmem[9] = 0x1200001B;//load b3 num to ECX
            cmem[10] = 0x20000003;//MUL ECX

            cmem[11] = 0x40000000;//add EAX, EBX 

            cmem[12] = 0x11000017;//load a4 num to EBX
            cmem[13] = 0x1200001C;//load b4 num to ECX
            cmem[14] = 0x20000003;//MUL ECX

            cmem[15] = 0x40000000;//add EAX, EBX 

            cmem[16] = 0x11000018;//load a5 num to EBX
            cmem[17] = 0x1200001D;//load b5 num to ECX
            cmem[18] = 0x20000003;//MUL ECX

            cmem[19] = 0x40000000;//add EAX, EBX 

            cmem[20] = 9456431; // 1 num a1
            cmem[21] = 9456434; // 2 num a2
            cmem[22] = 532; // 3 num a3
            cmem[23] = 44; // 4 num a4
            cmem[24] = 9456434; // 5 num a5

            cmem[25] = 4531; // 1 num  b1
            cmem[26] = 4531; // 2 num  b2
            cmem[27] = 822; // 3 num  b3
            cmem[28] = 19; // 4 num  b4
            cmem[29] = 51022; // 5 num  b5



            int countForLab3 = 0;
            for (int i = 0; i < cmem.Length; i++)
            {

                if (((cmem[i] >> 24) & 255) != 0)
                {
                    PC = i - countForLab3;
                    Console.WriteLine("PC: {0}", PC);
                    OpCode = DecodeOpCode(cmem[i]);
                    Console.WriteLine("OpCode: 0x{0:X}", OpCode);
                    value = cmem[cmem[i] & 4095];
                    //команды 
                    switch (OpCode)
                    {
                        case 0x10:
                            // load value to EAX
                            Load(ref EAX, value);
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x11:
                            // load value to EBX
                            Load(ref EBX, value);
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x12:
                            // load value to ECX
                            Load(ref ECX, value);
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x13:
                            // load value to EDX
                            Load(ref EDX, value);
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x20:
                            // add two registers
                            int regNumber1 = DefineReg1(cmem[i]);// номер регистра1
                            int regNumber2 = DefineReg2(cmem[i]);// номер регистра2
                            // sum of register values
                            int sumResult = Add(GetRegisterValue(regNumber1, EAX, EBX, ECX, EDX),
                                                 GetRegisterValue(regNumber2, EAX, EBX, ECX, EDX));
                            if (regNumber1 == 1)
                                EAX = sumResult;
                            else if (regNumber1 == 2)
                                EBX = sumResult;
                            else if (regNumber1 == 3)
                                ECX = sumResult;
                            else if (regNumber1 == 4)
                                EDX = sumResult;
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x30:
                            //mul ebx
                            long mulValue;
                            long sumValue;
                            if ((cmem[i] &4095) == 2)
                            {
                                if ((long) EAX * EBX > int.MaxValue)
                                {
                                    mulValue = EAX * EBX;
                                    EDX = (int) (mulValue - int.MaxValue);
                                    EAX = (int) (mulValue & int.MaxValue);
                                    
                                }
                                else EAX *= EBX;
                            }
                            //mul ecx
                            if ((cmem[i] & 4095) == 3)
                            {
                                if ((long)EBX * ECX > int.MaxValue)
                                {
                                    mulValue = EBX * ECX;
                                    ECX = (int)(mulValue - int.MaxValue);
                                    EBX = (int)(mulValue & int.MaxValue);

                                }
                                else EBX *= ECX;
                            }
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            break;
                        case 0x40:
                            if ((long)(EAX + EBX) > int.MaxValue)
                            {
                                sumValue = (long) EAX + EBX;
                                EAX = (int) sumValue * int.MaxValue;
                                CF = 1;
                                ADC(ref EDX, ref ECX, CF);
                            }
                            else
                            {
                                EAX = Add(EAX, EBX);
                                EDX = Add(EDX, ECX);
                            }
                                
                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                            ShowFlags(CF);
                            ResetFlags(CF);
                            break;
                    }
                }
                else
                    countForLab3++;


            }
            Console.WriteLine("Hex Result: 0x{0:X8}", EDX);
            Console.WriteLine("Hex EAX Result: 0x{0:X8}", EAX);
            Console.WriteLine("expcted result: {0:X16}", expectedResult);
            
        }

        static void ADC (ref int reg1, ref int reg2, int CF)
        {
            reg1 = reg1 + reg2 + CF;
        }

        static void ResetFlags(int CF)
        {
            CF = 0;
        }

        static void ShowFlags(int CF)
        {
            Console.WriteLine("       CF: {0}", CF);
        }

        // define number of reg1
        static int DefineReg1(int commandNumber)
        {
            return (commandNumber >> 12) & 4095;

        }
        // define number of reg2
        static int DefineReg2(int commandNumber)
        {

            return commandNumber & 4095;

        }
        // add registers values
        static int Add(int reg1, int reg2)
        {
            return reg1 += reg2;
        }
        // define register value
        static int GetRegisterValue(int regNum, int reg1, int reg2,
            int reg3, int reg4)
        {
            int registerValue = 0;

            switch (regNum)
            {
                case 1:
                    registerValue = reg1;
                    break;
                case 2:
                    registerValue = reg2;
                    break;
                case 3:
                    registerValue = reg3;
                    break;
                case 4:
                    registerValue = reg4;
                    break;
            }

            return registerValue;
        }

        //Load num to reg
        static void Load(ref int Reg, int value)
        {
            Reg = value;
        }
        // define command number
        static int DecodeOpCode(int commandNumber)
        {
            commandNumber = commandNumber >> 24;
            return commandNumber;
        }

        static void ShowRegisterValues(int EAX, int EBX,
            int ECX, int EDX)
        {
            //string sEAX = String.Format();
            Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
            Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
            Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
            Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
        }

        static int StrToIntOp1(string sOp1)
        {
            int op1 = 0;
            switch (sOp1)
            {
                case "EAX":
                    op1 = 4096; //2^12
                    break;
                case "EBX":
                    op1 = 8192; //2^13
                    break;
                case "ECX":
                    op1 = 8192 + 4096; //2^13 +2^12
                    break;
                case "EDX":
                    op1 = 16384; //2^14
                    break;
            }
            return op1;
        }

        static int StrToIntOp2(string sOp2)
        {
            int op2 = 0;

            switch (sOp2)
            {
                case "EAX":
                    op2 = 1; //2^0
                    break;
                case "EBX":
                    op2 = 2; //2^1
                    break;
                case "ECX":
                    op2 = 3; //2^1 + 2^0
                    break;
                case "EDX":
                    op2 = 4; //2^2
                    break;
            }
            return op2;
        }
    }
}