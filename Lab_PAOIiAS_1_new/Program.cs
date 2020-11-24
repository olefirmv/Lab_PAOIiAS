using System;
using System.Linq;


namespace Lab_PAOIiAS_1_new
{
    class Program
    {
        static uint EAX = 0 /*001*/, EBX = 0/*002*/, ECX = 0/*003*/, EDX = 0/*004*/;
        static uint EBP = 0/*005*/;
        static uint PC = 0;
        static uint OpCode;
        static uint resultCMD;
        static string tmpCmem;
        static uint tmpValue;
        static int arrLenght;

        static uint operand1;
        static uint operand2;
        static uint movRCRResult;
        static uint incResult;
        static uint movRVResult;
        static uint adcResult;
        static uint addResult;
        static uint subResult;
        
        static uint ZF = 0;
        static uint CF = 0;

        static uint[] cmem = new uint[250];
        //static uint[] arrNumbers;
        static uint[] arrCMDS;

        static void Main(string[] args)
        {
            for (int i = 0; i < cmem.Length; i++)
            {
                cmem[i] = 0;
            }
            //ArrInitLab1();
            //ArrInitLab2();

            ArrInitNumbers();
            ArrInitCmds();

            arrCMDS = tmpCmem.Substring(0, tmpCmem.Length - 1).Split(',').Select(value => uint.Parse(value)).ToArray();


            
            for (int i = 0; i < arrCMDS.Length; i++)
            {
                cmem[i] = arrCMDS[i];
            }

            //output cmds lab3

            for (int i = 0; i < arrCMDS.Length; i++)
            {
                Console.WriteLine("       0x{0:X8}", cmem[i]);
            }
            for (int i = 100; i < 101 + cmem[100]*2; i++)
            {
                Console.WriteLine("       0x{0:X8}", cmem[i]);
            }

            // cmds for lab1
            //cmem[0] = 0x10002064; // MovRV Ebx 100 
            //cmem[1] = 0x23002064; // AddRC EBX [100] 
            //cmem[2] = 0x10003065; // MovRV Ecx 101

            //cmem[3] = 0x11001003; // movRCR EAX cmem[ECX]
            //cmem[4] = 0x20004001; // AddRR EDX EAX  
            //cmem[5] = 0x31003002; // CMP ECX EBX   
            //cmem[6] = 0x22003000; // Inc ECX 
            //cmem[7] = 0x32003000; // JNE 3

            //cmds for lab2
            //cmem[0] = 0x10005064; //MovRV Ebp 100
            //cmem[1] = 0x23005064; //AddRC EBP [100] \1
            //cmem[2] = 0x10003065;//MovRV Ecx 101   \2

            //cmem[3] = 0x11001003;//movRCR EAX[ECX]\3
            //cmem[4] = 0x23003064;//AddRC ECX [100] \4
            //cmem[5] = 0x11002003;//movRCR EBX [ECX]\5
            //cmem[6] = 0x40002001;//Mul ebx eax        \6 - мл биты в eax, ст.биты в edx
            //cmem[7] = 0x240C8001;//AddCVR [200] EAX\7
            //cmem[8] = 0x210C9004;//Adc[201] EDX   \8
            //cmem[9] = 0x50003064;//Sub ECX, [100]  \9
            //cmem[10] = 0x31003005;//Cmp ECX, EBP    \10
            //cmem[11] = 0x22003000;//Inc ECX         \11
            //cmem[12] = 0x32003000;//Jne 3           \12

            //for lab1
            //tmpValue = 8;
            //for lab2
            //tmpValue = 13;
            //for lab3
            tmpValue =(uint) arrLenght;

            for (PC = 0; PC < tmpValue; PC++)
            {
                OpCode = DecodeOpCode(cmem[PC]);
                ShowPC(PC);        
                switch (OpCode)
                {
                    case 0x10:
                        // MovRV
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        movRVResult = Mov(GetRegisterValue(operand1), operand2);
                        setResultToReg1(operand1, movRVResult);
                        break;
                    case 0x11:
                        // MovRCR
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        movRCRResult = Mov(GetRegisterValue(operand1), cmem[GetRegisterValue(operand2)]);
                        setResultToReg1(operand1, movRCRResult);
                        break;
                    case 0x20:
                        //addRR
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        addResult = Add(GetRegisterValue(operand1), GetRegisterValue(operand2));
                        setResultToReg1(operand1, addResult);
                        break;
                    case 0x21:
                        //adc
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        adcResult = Adc(cmem[operand1], GetRegisterValue(operand2), CF);
                        setResultToCmem(operand1, adcResult);
                        break;
                    case 0x22:
                        // Inc
                        operand1 = DefineOp1(cmem[PC]);
                        incResult = Inc(GetRegisterValue(operand1));
                        setResultToReg1(operand1, incResult);
                        break;
                    case 0x23:
                        //addRC
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        addResult = Add(GetRegisterValue(operand1), cmem[operand2]);
                        setResultToReg1(operand1, addResult);
                        break;
                    case 0x24:
                        //addCVR
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        addResult = Add(cmem[operand1], GetRegisterValue(operand2));
                        setResultToCmem(operand1, addResult);
                        break;
                    case 0x31:
                        //cmp 
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        int resultCMP = (int)(GetRegisterValue(operand1) - GetRegisterValue(operand2));
                        if (resultCMP == 0)
                            ZF = 1;
                        else ZF = 0;
                        break;
                    case 0x32:
                        //jne
                        operand1 = DefineOp1(cmem[PC]);
                        if (ZF == 0)
                        {
                            PC = operand1 -1;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 0x40:
                        //mul
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        Mul(GetRegisterValue(operand1), GetRegisterValue(operand2));
                        break;
                    case 0x50:
                        //sub
                        operand1 = DefineOp1(cmem[PC]);
                        operand2 = DefineOp2(cmem[PC]);
                        subResult = Sub(GetRegisterValue(operand1), cmem[operand2]);
                        setResultToReg1(operand1, subResult);
                        break;
                }
                ShowRegisterValues();
            }
        }
        //for lab1
        static uint[] ArrInitLab1()
        {
            Console.WriteLine("Enter length of arr:");
            string arrLenghtStr = Console.ReadLine();
            int arrLenght = int.Parse(arrLenghtStr);
            cmem[100] = (uint) arrLenght;
            for (int i = 101; i < 101 + arrLenght; i++)
            {
                Console.WriteLine("Enter {0} value:", i);
                cmem[i] = uint.Parse(Console.ReadLine());
            }
            return cmem;
        }
        //for lab2
        static uint[] ArrInitLab2()
        {
            uint[] a = new uint[] { 5, 0xFFFF0000, 3, 0xFFFF0000 };
            uint[] b = new uint[] { 5, 2, 3, 5 };
            
            uint arrLenght =(uint) a.Length;
            cmem[100] = arrLenght;
            for (int i = 0; i < arrLenght*2; i++)
            {
                if (i > arrLenght - 1)
                {
                    cmem[101 + i] = b[i - arrLenght];
                }
                else
                {
                    cmem[101 + i] = a[i];
                }
            }
            return cmem;
        }
        //for lab3
        
        #region Lab3
        static void ArrInitNumbers()
        {
            Console.WriteLine("How many arrays do you want to enter");
            int qArr = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter length of arr:");
            string arrLenghtStr = Console.ReadLine();
            
            arrLenght = int.Parse(arrLenghtStr) * qArr;
            cmem[100] = uint.Parse(arrLenghtStr);
            for (int i = 0; i < arrLenght; i++)
            {
                Console.WriteLine("Enter {0} value:", i);
                cmem[101 + i] = uint.Parse(Console.ReadLine());
            }

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
                        cmdType = cmdTypeToInt (28,0,0);// 2^28
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = uint.Parse(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "MovRCR")
                    {
                        sCmdType = "0x11";
                        cmdType = cmdTypeToInt(28, 24, 0);// 2^28
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "AddRR")
                    {
                        sCmdType = "0x20";
                        cmdType = cmdTypeToInt(29, 0, 0);// 2^29
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Adc")
                    {
                        sCmdType = "0x21";
                        cmdType = cmdTypeToInt(29, 24, 0);// 2^29
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Inc")
                    {
                        sCmdType = "0x22";
                        cmdType = cmdTypeToInt(29, 25, 0);// 2^29+ 2^24
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = 0;
                    }
                    if (arrayCmd[0] == "AddRC")
                    {
                        sCmdType = "0x23";
                        cmdType = cmdTypeToInt(29, 25, 24);//
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "AddCVR")
                    {
                        sCmdType = "0x24";
                        cmdType = cmdTypeToInt(29, 26, 0);// 2^29
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Cmp")
                    {
                        sCmdType = "0x31";
                        cmdType = cmdTypeToInt(29, 28, 24);
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Jne")
                    {
                        sCmdType = "0x32";
                        cmdType = cmdTypeToInt(29, 28, 25);
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = 0;
                    }
                    if (arrayCmd[0] == "Mul")
                    {
                        sCmdType = "0x40";
                        cmdType = cmdTypeToInt(30, 0, 0);
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    if (arrayCmd[0] == "Sub")
                    {
                        sCmdType = "0x50";
                        cmdType = cmdTypeToInt(30, 28, 0);
                        op1 = DefineOp1StrToInt(arrayCmd[1]);
                        op2 = DefineOp2StrToInt(arrayCmd[2]);
                    }
                    resultCMD = cmdType + op1 + op2;
                    tmpCmem += resultCMD + ",";
                }
            }
        }

        static uint DefineOp1StrToInt(string op)
        {
            char[] smbsToTrim = new char[] { '[', ']' };
            op = op.Trim(smbsToTrim);
            switch (op)
            {
                case "EAX": return 4096; //2^12;
                case "EBX": return 8192; //2^13
                case "ECX": return 8192 + 4096; //2^13 +2^12
                case "EDX": return 16384; //2^14
                case "EBP": return cmdTypeToInt(14,12,0);
                default: return CalculateOp1(int.Parse(op));
            }
        }
        static uint DefineOp2StrToInt(string op)
        {
            char[] smbsToTrim = new char[] { '[', ']' };
            op = op.Trim(smbsToTrim);
            switch (op)
            {
                case "EAX": return 1;
                case "EBX": return 2;
                case "ECX": return 3;
                case "EDX": return 4;
                case "EBP": return 5;
                default: return uint.Parse(op);
            }
        }
        
        static uint cmdTypeToInt (int n1, int n2, int n3)
        {
            if (n2 == 0 && n3 == 0)
            {
                return Convert.ToUInt32(Math.Pow(2, n1));
            }
            if (n3 == 0)
            {
                return Convert.ToUInt32(Math.Pow(2, n1) + Math.Pow(2, n2));
            }
            else return Convert.ToUInt32(Math.Pow(2, n1) + Math.Pow(2, n2) + Math.Pow(2, n3));
            
        }

        static uint CalculateOp1(int value)
        {
            double result = 0;
            for (int i = 0; i < 12; i++)
            {
                int number = value & 1;
                double res = number * Math.Pow(2, 12 + i);
                result += res;
                value = value >> 1;
            }
            return Convert.ToUInt32(result); 
        }
        #endregion 


        static void setResultToReg1(uint op1, uint opResult)
        {
            if (op1 == 1)
                EAX = opResult;
            else if (op1 == 2)
                EBX = opResult;
            else if (op1 == 3)
                ECX = opResult;
            else if (op1 == 4)
                EDX = opResult;
            else if (op1 == 5)
                EBP = opResult;
        }
        static void setResultToCmem (uint op1, uint opResult)
        {
            cmem[op1] = opResult;
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
                case 5:
                    registerValue = EBP;
                    break;
            }
            return registerValue;
        }

        static uint Add(uint op1, uint op2)
        {
            long sumVal = (long) op1 + op2;
            if (sumVal > 0xFFFFFFFF)
            {
                CF = 1;
                return op1 = (uint)(sumVal & 0xFFFFFFFF);
            }
            else
            {
                CF = 0;
                return op1 += op2;
            }          
        }

        static void Mul(uint op1, uint op2)
        {
            long mulValue = (long)op1 * op2;
            if (mulValue > 0xFFFFFFFF)
            {
                uint tmp = (uint)(mulValue >> 32) & 0xFFFFFFFF;
                EAX = (uint)(mulValue & 0xFFFFFFFF);
                EDX = tmp;
            }
            else
            {
                EDX = 0;
                EAX = op1 * op2;
            }
        }
        static uint Sub(uint op1, uint op2)
        {
            return op1 -= op2;
        }

        static uint Adc(uint reg1, uint reg2, uint flag)
        {
            return reg1 + reg2 + flag;
        }
        static uint Inc(uint reg)
        {
            return ++reg;
        }

        static uint Mov(uint reg, uint value)
        {
            return reg = value;
        }

        static uint DecodeOpCode(uint cmd)
        {
            return cmd >> 24;
        }
        // for lab1
        //static void ShowRegisterValues()
        //{
        //        Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
        //        Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
        //        Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
        //        Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
        //        Console.WriteLine("       flag: {0}", ZF);
        //}

        //for lab2,3
        static void ShowRegisterValues()
        {
            Console.WriteLine("       register EAX: 0x{0:X8}", EAX);
            Console.WriteLine("       register EBX: 0x{0:X8}", EBX);
            Console.WriteLine("       register ECX: 0x{0:X8}", ECX);
            Console.WriteLine("       register EDX: 0x{0:X8}", EDX);
            Console.WriteLine("       register EBP: 0x{0:X8}", EBP);
            Console.WriteLine("       cmem [201]: 0x{0:X8}", cmem[201]);
            Console.WriteLine("       cmem [200]: 0x{0:X8}", cmem[200]);
            Console.WriteLine("       flag CF: {0}", CF);
            Console.WriteLine("       flag ZF: {0}", ZF);
        }

        static void ShowPC (uint PC)
        {
                Console.WriteLine("PC:{0}", PC);
                Console.WriteLine("       OpCode: 0x{0:X}", OpCode);
        }
    }
}
