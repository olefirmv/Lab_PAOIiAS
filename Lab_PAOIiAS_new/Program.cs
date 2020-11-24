using System;
using System.Linq;

namespace Lab_PAOIiAS_3_new
{
    class Program
    {
        
        static uint EAX = 0 /*001*/, EBX = 0/*002*/, ECX = 0/*003*/, EDX = 0/*004*/;
        static uint EBP = 0/*005*/, ESP = 0/*006*/, ESI = 0/*007*/;
        static uint PC = 0;
        static uint OpCode;
        static uint resultCMD;
        static string tmpCmem;
        static uint tmpValue;
        static uint regNumber1;
        static uint regNumber2;
        static uint movRCRResult;
        static uint incResult;
        static uint movRVResult;
        static uint index;
        static uint ZF = 0;

        static uint[] cmem = new uint[200];
        static uint[] arrNumbers;
        static uint[] arrCMDS;

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

            arrCMDS = tmpCmem.Substring(0,tmpCmem.Length-1).Split(',').Select(value => uint.Parse(value)).ToArray();

            
            cmem[100] = (uint) arrNumbers.Length;
            for(int i = 0; i < arrCMDS.Length;i ++)
            {
                cmem[i] = arrCMDS[i];
                
            }
            for (int i = 0; i < arrNumbers.Length; i++)
            {
                cmem[i +101] = arrNumbers[i];
            }
                  
            for (PC = 0; PC < 8; PC++)
            {
                
                switch (OpCode)
                {
                    case 0x10:
                        // MovRV
                        regNumber1 = DefineOp1(cmem[PC]);
                        uint value = DefineOp2(cmem[PC]);
                        movRVResult = MovRV(GetRegisterValue(regNumber1), value);
                        setResultToReg1(regNumber1, movRVResult);
                        break;
                    case 0x11:
                        // MovRCe
                        regNumber1 = DefineOp1(cmem[PC]);
                        regNumber2 = DefineOp2(cmem[PC]);
                        movRCRResult = MovRCR(GetRegisterValue(regNumber1), cmem[GetRegisterValue(regNumber2)]);
                        setResultToReg1(regNumber1, movRCRResult);
                        break;
                    case 0x20:
                        //addRR
                        regNumber1 = DefineOp1(cmem[PC]);
                        regNumber2 = DefineOp2(cmem[PC]);
                        uint sumResult = AddRR(GetRegisterValue(regNumber1), GetRegisterValue(regNumber2));
                        setResultToReg1(regNumber1, sumResult);
                        break;
                    case 0x23:
                        //addRC
                        regNumber1 = DefineOp1(cmem[PC]);
                        uint value2 = DefineOp2(cmem[PC]);
                        sumResult = AddRC(GetRegisterValue(regNumber1), value2);
                        setResultToReg1(regNumber1, sumResult);
                        break;
                    case 0x22:
                        // Inc
                        regNumber1 = DefineOp1(cmem[PC]);
                        incResult = Inc(GetRegisterValue(regNumber1));
                        setResultToReg1(regNumber1, incResult);
                        break;
                    case 0x31:
                        //cmp 
                        regNumber1 = DefineOp1(cmem[PC]);
                        regNumber2 = DefineOp2(cmem[PC]);
                        int resultCMP = (int)(GetRegisterValue(regNumber1) - GetRegisterValue(regNumber2));
                        if (resultCMP == 0)
                            ZF = 1;
                        else ZF = 0;
                        break;
                    case 0x32:
                        //jne
                        if (ZF == 0)
                        {
                            PC = 2;
                            break;
                        }
                        else
                        {
                            break;
                        }
                }
                ShowRegisterValues();
                
            }

        }
        static uint GetRegisterValue(uint regNum)
        {
            uint registerValue = 0;

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

        static void setResultToReg1(uint op1, uint opResult )
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
                    uint cmdType = 0;
                    uint op1 = 0;
                    uint op2 = 0;
                    if (arrayCmd[0] == "MovRV")
                    {
                        sCmdType = "0x10";
                        cmdType = 268435456;// 2^28
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = uint.Parse(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "MovRCR")
                    {
                        sCmdType = "0x11";
                        cmdType = 268435456;// 2^28
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = uint.Parse(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Cmp")
                    {
                        sCmdType = "0x31";
                        cmdType = 536870912 + 268435456 + 16777216;// 2^29 + 2^28 + 2^24
                        op1 = 0;
                        op2 = 0;
                    }
                    if (arrayCmd[0] == "Jne")
                    {
                        sCmdType = "0x32";
                        cmdType = 268435456;// 2^28
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = uint.Parse(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Mov")
                    {
                        sCmdType = "0x11";
                        cmdType = 268435456 + 16777216;// 2^28 + 2^24
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = DefineReg2Mov(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "AddRR")
                    {
                        sCmdType = "0x20";
                        cmdType = 536870912;// 2^29
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = DefineReg2StrToInt(arrayCmd[2]);
                    }
                    if(arrayCmd[0] == "Inc")
                    {
                        sCmdType = "0x22";
                        cmdType = 536870912 + 16777216;// 2^29+ 2^24
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = 1;
                    }
                    if (arrayCmd[0] == "AddRR")
                    {
                        sCmdType = "0x23";
                        cmdType = 536870912;// 2^29
                        op1 = DefineReg1StrToInt(arrayCmd[1]);
                        op2 = DefineReg2StrToInt(arrayCmd[2]);
                    }

                    resultCMD = cmdType + op1 + op2;
                    tmpCmem += resultCMD + ",";
                    
                }
            }
        }

        // define number of reg1
        static uint DefineOp1(uint commandNumber)
        {
            return (commandNumber >> 12) & 4095;

        }
        // define number of reg2
        static uint DefineOp2(uint commandNumber)
        {

            return commandNumber & 4095;

        }
       
        static uint DefineReg2StrToInt(string op)
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
        static uint DefineReg2Mov(string op)
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
                cmem[100] = (uint)arrLenght;
                for (int i = 101; i < 101 + arrLenght; i++)
                {
                    Console.WriteLine("Enter {0} value:", i);
                    cmem[i] = uint.Parse(Console.ReadLine());
                }
                
        }
        static uint DefineReg1StrToInt (string op)
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
        static uint AddRR(uint reg1, uint op2)
        {
            return reg1 += op2;
        }
        static uint AddRC(uint reg1, uint op2)
        {
            return reg1 += cmem[op2];

        }

        static uint Inc(uint reg)
        {
            return ++reg;
        }

        static uint MovRCR(uint reg, uint value)
        {
            return reg = value;
        }

        static uint MovRV(uint reg, uint value)
        {
            return reg = value;
        }
        static uint DecodeOpCode(uint cmd)
        {
            return cmd >> 24;
        }
        
        static void ShowRegisterValues()
        {
            Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
            Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
            Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
            Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
            Console.WriteLine("       flag: {0}", ZF);
        }
        static void ShowPC()
        {
            Console.WriteLine("PC:{0}", PC);
            Console.WriteLine("       OpCode: 0x{0:X}", OpCode);
        }
    }
}
