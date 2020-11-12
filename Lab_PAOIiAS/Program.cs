using System;


namespace Lab_PAOIiAS_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //регистры
            int EAX = 0, EBX = 0, ECX = 0, EDX = 0;
            
            //value
            int value;
            // счетчик команд, команд операций
            int PC = 0, OpCode;

            //массив чисел для сложения
            int[] numbers = new int[] { 5, 3, 1, 6, 10 };
            int expectedResult = 0;
            for (int i=0; i < numbers.Length; i++)
            {
                expectedResult += numbers[i];
            }
            Console.WriteLine("Expected result : {0}",expectedResult);
            //оперативная память
            int[] cmem = new int[14];
            cmem[0] = 0x10000009;//load 1 num to EAX 
            cmem[1] = 0x1100000A;//load 2 num to EBX
            cmem[2] = 0x1200000B;//load 3 num to ECX
            cmem[3] = 0x1300000C;//load 4 num to EDX
            cmem[4] = 0x20001002;//add EAX, EBX
            cmem[5] = 0x20003004;//add ECX, EDX
            cmem[6] = 0x20001003;//add EAX, ECX
            cmem[7] = 0x1100000D;//load 5 num to EBX
            cmem[8] = 0x20001002;//add EAX, EBX

            cmem[9] = 0x00000005; // 1 num
            cmem[10] = 0x00000003; // 2 num
            cmem[11] = 0x00000001; // 3 num
            cmem[12] = 0x00000006; // 4 num
            cmem[13] = 0x0000000A; // 5 num


            for (PC = 0; PC < cmem.Length; PC++)
            {
                if (((cmem[PC]>>24) & 255) != 0 )
                {
                    Console.WriteLine("PC: {0}", PC);
                    OpCode = DecodeOpCode(cmem[PC]);
                    Console.WriteLine("OpCode: 0x{0:X}", OpCode);
                    value = cmem[cmem[PC] & 4095];
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
                            int regNumber1 = DefineReg1(cmem[PC]);// номер регистра1
                            int regNumber2 = DefineReg2(cmem[PC]);// номер регистра2
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
                    }
                } 
            }
            Console.WriteLine("Hex Result: 0x{0:X8}", EAX);
            Console.WriteLine("Int Result: {0}", EAX & 4095);
            if ((EAX & 4095) == expectedResult)
                Console.WriteLine("Register value equals the expected result");
            
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
    }
}