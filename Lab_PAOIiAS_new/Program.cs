using System;
using System.Linq;

namespace Lab_PAOIiAS_3_new
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

            //ArrInit();
            //cmds
            //cmem[0] = 0x10003007; // load 1st index in cmem to ECX
            //cmem[1] = 0x30000000;// L1
            //cmem[2] = 0x11001003;// mov EAX cmem[ECX]
            //cmem[3] = 0x20004001;// add two registers
            //cmem[4] = 0x21003001;// Inc ECX 1
            //cmem[5] = 0x31000000;// loop

            //int tmpValue = 10 + cmem[9];

            ArrInitNumbers();
            ArrInitCmds();

            arrCMDS = tmpCmem.Substring(0,tmpCmem.Length-1).Split(',').Select(value => int.Parse(value)).ToArray();

            cmem = new int[arrCMDS.Length + 1 + arrNumbers.Length];
            cmem[arrCMDS.Length] = arrNumbers.Length;
            for(int i = 0; i < arrCMDS.Length;i ++)
            {
                cmem[i] = arrCMDS[i];
                
            }
            for (int i = 0; i < arrNumbers.Length; i++)
            {
                cmem[i + arrCMDS.Length +1] = arrNumbers[i];
            }
            
            bool tmpFlag = true;
            
            for(int i = 0; i < cmem.Length; i++ )
            {
                if(((cmem[i] >> 24) & 255) == 0x30 || ((cmem[i] >> 24) & 255) == 0x31)
                {
                    tmpFlag = false;
                }
                Console.WriteLine(": 0x{0:X8}",cmem[i]);
            }

            if (tmpFlag == false)
                tmpValue = cmem.Length;
            else tmpValue = arrCMDS.Length;


            while (ECX != tmpValue)
            {
                if (tmpFlag == true && PC < arrCMDS.Length)
                {
                    OpCode = DecodeOpCode(cmem[PC]);
                    ShowInfo();
                }
                else if (tmpFlag == false)
                {
                    OpCode = DecodeOpCode(cmem[PC]);
                    ShowInfo();
                }
                else break;

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
                    case 0x21:
                        // Inc
                        regNumber1 = DefineReg1(cmem[PC]);
                        incResult = Inc(GetRegisterValue(regNumber1));
                        setResultToReg1(regNumber1, incResult);
                        break;
                    case 0x31:
                        Console.WriteLine("Loop L1");
                        //loop
                        PC = index;
                        break;
                }
                ShowRegisterValues();
                PC++;
            }

            if (tmpFlag == false)
            {
                OpCode = DecodeOpCode(cmem[PC]);
                ShowInfo();
                Console.WriteLine("Loop L1");
                ShowRegisterValues();
            }
        }

        static void setResultToReg1(int op1, int opResult )
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

        static void ArrInitCmds()
        {
            bool inputFlag = true;
            tmpCmem = String.Empty;
            while (inputFlag)
            {
                Console.WriteLine("Enter the command:");
                string cmd = Console.ReadLine();
                string[] arrayCmd = cmd.Split(' ');
                if (arrayCmd[0] == "stop")
                {
                    break;
                }
                else
                {
                    string sCmdType = "";
                    string sOp1 = "";
                    string sOp2 = "";
                    int cmdType = 0;
                    int op1 = 0;
                    int op2 = 0;
                    if (arrayCmd[0] == "Load")
                    {
                        sCmdType = "0x10";
                        cmdType = 268435456;// 2^28
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = int.Parse(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "L1")
                    {
                        sCmdType = "0x30";
                        cmdType = 536870912 + 268435456;// 2^29+ 2^28
                        op1 = 0;
                        op2 = 0;
                    }
                    if (arrayCmd[0] == "Loop")
                    {
                        sCmdType = "0x31";
                        cmdType = 536870912 + 268435456 + 16777216;// 2^29 + 2^28 + 2^24
                        op1 = 0;
                        op2 = 0;
                    }
                    if (arrayCmd[0] == "Mov")
                    {
                        sCmdType = "0x11";
                        cmdType = 268435456 + 16777216;// 2^28 + 2^24
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = DefineReg2Mov(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Add")
                    {
                        sCmdType = "0x20";
                        cmdType = 536870912;// 2^29
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = DefineReg2StrToInt(arrayCmd[2]);
                    }
                    if(arrayCmd[0] == "Inc")
                    {
                        sCmdType = "0x21";
                        cmdType = 536870912 + 16777216;// 2^29+ 2^24
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = 1;
                    }
                    
                    resultCMD = cmdType + op1 + op2;
                    tmpCmem += resultCMD + ",";
                    
                }
            }
        }
       
        static int DefineReg2StrToInt(string op)
        {
            switch (op)
            {
                case "EAX": return 1; 
                case "EBX": return 2; 
                case "ECX": return 3; 
                case "EDX": return 4; 
                default: return 0;
            }
        }
        static int DefineReg2Mov(string op)
        {
            char[] smbsToTrim = new char[] {'[', ']' };
            op = op.Trim(smbsToTrim);
            switch (op)
            {
                case "EAX": return 1;
                case "EBX": return 2;
                case "ECX": return 3;
                case "EDX": return 4;
                default: return 0;
            }
        }
        static void ArrInitNumbers()
        {

            Console.WriteLine("Enter length of arr:");
            string arrLenghtStr = Console.ReadLine();
            int arrLenght = int.Parse(arrLenghtStr);
            arrNumbers = new int[arrLenght];
            
            for (int i = 0; i < arrLenght; i++)
            {
                Console.WriteLine("Enter {0} value:", i + 1);
                arrNumbers[i] = int.Parse(Console.ReadLine());
            }
            
        }
        static int DefineReg1StrToInt (string op)
        {
            switch(op)
            {
                case "EAX": return 4096; //2^12;
                case "EBX": return 8192; //2^13
                case "ECX": return 8192 + 4096; //2^13 +2^12
                case "EDX": return 16384; //2^14
                default: return 0;
            }
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

        static int Add( int reg1, int reg2)
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
        static void ShowRegisterValues()
        {
            Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
            Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
            Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
            Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
        }
        static void ShowInfo()
        {
            Console.WriteLine("PC:{0}", PC);
            Console.WriteLine("       OpCode: 0x{0:X}", OpCode);
        }
    }
}
