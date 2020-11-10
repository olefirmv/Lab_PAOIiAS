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
            int PC, OpCode;

            //массив чисел для сложения
            int[] numbers = new int[] { 5, 3, 1, 6, 10 };
            int expectedResult = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                expectedResult += numbers[i];
            }
            Console.WriteLine("Expected result : {0}", expectedResult);
            //оперативная память
            int[] cmem = new int[14];
            cmem[0] = 0x10000009;//load 1 num to Eax 
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


            bool inputFlag = true;

            while (inputFlag)
            {
                Console.WriteLine("Enter the command:");
                string cmd = Console.ReadLine();
                string[] arrayCmd = cmd.Split(' ');
                if (arrayCmd[0] == "stop")
                {
                    inputFlag = false;
                }
                else
                {
                    string sCmdType = "";
                    string sOp1 = "";
                    string sOp2 = "";
                    int cmdType = 0;
                    int op1 = 0;
                    int op2 = 0;
                    int resultCMD = 0;

                    if (arrayCmd[0] == "Load")
                    {
                        if (arrayCmd[1] == "EAX")
                        {
                            sCmdType = "0x10";
                            cmdType = 268435456;// 2^28
                        }
                        if (arrayCmd[1] == "EBX")
                        {
                            sCmdType = "0x11";
                            cmdType = 268435456 + 16777216;// 2^28 + 2^24
                        }
                        if (arrayCmd[1] == "ECX")
                        {
                            sCmdType = "0x12";
                            cmdType = 268435456 + 16777216;// 2^28 + 2^25
                        }
                        if (arrayCmd[1] == "EDX")
                        {
                            sCmdType = "0x13";
                            cmdType = 268435456 + 33554432 + 16777216;// 2^28 + 2^25 + 2^24
                        }
                        sOp1 = "000";
                        sOp2 = arrayCmd[2];
                        op1 = Convert.ToInt32(sOp1);
                        op2 = Convert.ToInt32(sOp2);

                        Array.Resize<int>(ref cmem, cmem.Length + 2);

                        cmem[cmem.Length - 2] = op2;
                        resultCMD = cmdType + op1 + cmem.Length - 2;
                        cmem[cmem.Length - 1] = resultCMD;

                    }
                    if (arrayCmd[0] == "Add")
                    {
                        sCmdType = "0x20";
                        cmdType = 536870912;// 2^29
                        sOp1 = arrayCmd[1];
                        sOp2 = arrayCmd[2];
                        op1 = StrToIntOp1(sOp1);
                        op2 = StrToIntOp2(sOp2);
                        resultCMD = cmdType + op1 + op2;

                        Array.Resize<int>(ref cmem, cmem.Length + 1);

                        cmem[cmem.Length - 1] = resultCMD;
                    }

                }
            }

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
                    }
                }
                else
                    countForLab3++;


            }
            Console.WriteLine("Hex Result: 0x{0:X8}", EAX);
            Console.WriteLine("Int Result: {0}", EAX & 4095);
            if ((EAX & 4095) == expectedResult)
                Console.WriteLine("Register value equals the expected result");
            Console.WriteLine("add command hex - 0x{0:X8}", cmem[cmem.Length - 1]);
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