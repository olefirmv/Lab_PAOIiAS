using System;


namespace Lab_PAOIiAS_1_new
{
    class Program
    {
        static int EAX = 0 /*001*/, EBX = 0/*002*/, ECX = 0/*003*/, EDX = 0/*004*/;
        static int PC = 0;
        static int OpCode;
        static int resultCMD;
        static string tmpCmem;
        static int tmpValue;
        static int regNumber1;
        static int regNumber2;
        static int movResult;
        static int incResult;
        static int loadResult;
        static int index;

        static int[] cmem;
        static int[] arrNumbers;
        static int[] arrCMDS;

        static void Main(string[] args)
        {
            foreach (uint i in cmem)
            {
                cmem[i] = 0;
            }
            ArrInit();
            //cmds
            cmem[0] = 0x10003007; // load 1st index in cmem to ECX
            cmem[1] = 0x30000000;// L1
            cmem[2] = 0x11001003;// mov EAX cmem[ECX]
            cmem[3] = 0x20004001;// add two registers
            cmem[4] = 0x22003001;// Inc ECX 1
            cmem[5] = 0x31000000;// loop

            int tmpValue = 6;
            

            while (PC != tmpValue)
            {
                OpCode = DecodeOpCode(cmem[PC]);
                ShowPC(PC);        
                switch (OpCode)
                {
                    case 0x10:
                        // load
                        regNumber1 = DefineReg1(cmem[PC]);
                        int value = DefineReg2(cmem[PC]);
                        loadResult = Load(GetRegisterValue(regNumber1), value);
                        setResultToReg1(regNumber1, loadResult);
                        break;
                    case 0x30:
                        //L1
                        Console.WriteLine("L1");
                        index = PC - 1;
                        break;
                    case 0x11:
                        // mov
                        regNumber1 = DefineReg1(cmem[PC]);
                        regNumber2 = DefineReg2(cmem[PC]);
                        movResult = Mov(GetRegisterValue(regNumber1), cmem[GetRegisterValue(regNumber2)]);
                        setResultToReg1(regNumber1, movResult);
                        break;
                    case 0x20:
                        //add two registers
                        regNumber1 = DefineReg1(cmem[PC]);
                        regNumber2 = DefineReg2(cmem[PC]);
                        int sumResult = Add(GetRegisterValue(regNumber1), GetRegisterValue(regNumber2));
                        setResultToReg1(regNumber1, sumResult);
                        break;
                    case 0x22:
                        // Inc
                        regNumber1 = DefineReg1(cmem[PC]);
                        incResult = Inc(GetRegisterValue(regNumber1));
                        setResultToReg1(regNumber1, incResult);
                        break;
                    case 0x31:
                        //loop
                        if (ECX == cmem.Length)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Loop L1");
                            PC = index;
                            break;
                        }
                }
                ShowRegisterValues(EAX, EBX, ECX, EDX);
                PC++; 
            }

            OpCode = DecodeOpCode(cmem[PC]);
            ShowPC(PC);
            Console.WriteLine("Loop L1");
            ShowRegisterValues(EAX, EBX, ECX, EDX);
        }

        static int[] ArrInit()
        {
            Console.WriteLine("Enter length of arr:");
            string arrLenghtStr = Console.ReadLine();
            int arrLenght = int.Parse(arrLenghtStr);
            cmem = new int[7 + arrLenght];
            cmem[6] = arrLenght;
            for (int i = 0; i < arrLenght; i++)
            {
                Console.WriteLine("Enter {0} value:", i + 1);
                cmem[7 + i] = int.Parse(Console.ReadLine());
            }
            return cmem;
        }
        static void setResultToReg1(int op1, int opResult)
        {
            if (op1 == 1)
                EAX = opResult;
            else if (op1 == 2)
                EBX = opResult;
            else if (op1 == 3)
                ECX = opResult;
            else if (op1 == 4)
                EDX = opResult;
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

        static int GetRegisterValue(int regNum)
        {
            int registerValue = 0;

            switch (regNum)
            {
                case 1:
                    registerValue = EAX;
                    break;
                case 2:
                    registerValue = EBX;
                    break;
                case 3:
                    registerValue = ECX;
                    break;
                case 4:
                    registerValue = EDX;
                    break;
            }

            return registerValue;
        }

        static int Add(int reg1, int reg2)
        {
            return reg1 += reg2;
        }

        static int Inc(int reg)
        {
            return reg = reg + 1;
        }

        static int Mov(int reg, int value)
        {
            return reg = value;
        }

        static int Load(int reg, int value)
        {
            return reg = value;
        }
        static int DecodeOpCode(int cmd)
        {
            return cmd >> 24;
        }
        static void ShowRegisterValues(int EAX, int EBX, int ECX, int EDX)
        {
                Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
                Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
                Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
                Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
        }
        static void ShowPC (int PC)
        {
                Console.WriteLine("PC:{0}", PC);
                Console.WriteLine("       OpCode: 0x{0:X}", OpCode);
        }
    }
}
