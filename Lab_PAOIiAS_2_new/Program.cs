using System;


namespace Lab_PAOIiAS_1_new
{
    class Program
    {
        static uint EAX = 0 /*001*/, EBX = 0/*002*/, ECX = 0/*003*/, EDX = 0/*004*/;
        static uint EBP = 0/*005*/, ESP = 0/*006*/, ESI = 0/*007*/;
        static uint CF;
        static uint PC = 0;
        static uint OpCode;
        static uint[] cmem = new uint[200];

        static void Main(string[] args)
        {
            foreach (uint i in cmem)
            {
                cmem[i] = 0;
            }
            ArrInit();
            //cmds
            cmem[0] = 0x1000700B;// load 1st index in cmem to ESI
            cmem[1] = 0x30000000;// L1
            cmem[2] = 0x11101007;// mov EAX [esi]
            cmem[3] = 0x11202007;// mov EBX [esi + cmem[11]]
            cmem[4] = 0x40001002;// MUL EBX
            cmem[5] = 0x20006004;// add esp edx = lsb
            cmem[6] = 0x21005003;// adc ebp ecx = msb
            cmem[7] = 0x22003001;// Inc ESI 1
            cmem[8] = 0x31000000;// loop


            uint tmpValue = 10 + cmem[9];

            for (int i = 0; i < cmem.Length; i++)
            {
                OpCode = DecodeOpCode(cmem[PC]);
                ShowInfo();

                switch (OpCode)
                {
                    case 0x10:
                        // load
                        Load(ref ESI, 10);
                        break;
                    case 0x30:
                        //L1 
                        Console.WriteLine("L1");
                        break;
                    case 0x11:
                        // mov
                        if (((cmem[PC] >> 20) & 15) == 1)
                            Mov(ref EAX, cmem[ESI]);
                        else
                            Mov(ref EBX, cmem[ESI + cmem[9]]);
                        break;
                    case 0x40:
                        //mul
                        Mul(EAX, EBX);
                        break;
                    case 0x20:
                        //add
                        Add(ref ESP, EDX);
                        break;
                    case 0x21:
                        //adc
                        Adc(ref EBP, ECX, CF);
                        break;
                    case 0x22:
                        // Inc
                        Inc(ref ESI);
                        break;
                    case 0x31:
                        Console.WriteLine("Loop L1");
                        //loop //5
                        PC = 0;
                        break;
                }

                ShowRegisterValues();
                Console.WriteLine("       CF:{0}", CF);
                PC++;
            }


            OpCode = DecodeOpCode(cmem[PC]);
            ShowInfo();
            Console.WriteLine("Loop L1");
            ShowRegisterValues();
        }

        static uint[] ArrInit()
        {

            uint[] a = new uint[] { 5, 0xFFFF0000, 3, 0xFFFF0000 };
            uint[] b = new uint[] { 5, 2, 3, 5 };
            uint arrLenght = (uint)a.Length;
            cmem = new uint[9 + 1 + 2 * arrLenght];
            cmem[9] = arrLenght;
            for (int i = 0; i < arrLenght * 2; i++)
            {
                if (i > arrLenght - 1)
                {
                    cmem[10 + i] = b[i - arrLenght];
                }
                else
                {
                    cmem[10 + i] = a[i];
                }

            }
            return cmem;
        }

        static void Mul(uint reg1, uint reg2)
        {
            long mulValue = (long)reg1 * reg2;
            if (mulValue > 0xFFFFFFFF)
            {
                uint tmp = (uint)(mulValue >> 32) & 0xFFFFFFFF;
                EDX = (uint)(mulValue & 0xFFFFFFFF);
                ECX = tmp;
            }
            else
            {
                ECX = 0;
                EDX = reg1 * reg2;

            }
        }
        static void Add(ref uint reg1, uint reg2)
        {
            long sumVal = (long)reg1 + reg2;
            if (sumVal > 0xFFFFFFFF)
            {
                CF = 1;
                reg1 = (uint)(sumVal & 0xFFFFFFFF);
            }
            else
            {
                CF = 0;
                reg1 += reg2;
            }
        }
        static void Adc(ref uint reg1, uint reg2, uint flag)
        {
            reg1 = reg1 + reg2 + flag;
        }

        static void Inc(ref uint reg)
        {
            reg = reg + 1;
        }

        static void Mov(ref uint reg, uint value)
        {
            reg = value;
        }

        static void Load(ref uint reg, uint firstNumberIndex)
        {
            reg = firstNumberIndex;
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
            Console.WriteLine("       register EBP: 0x{0:X8}", EBP);
            Console.WriteLine("       register ESP: 0x{0:X8}", ESP);
            Console.WriteLine("       register ESI: 0x{0:X8}", ESI);
        }
        static void ShowInfo()
        {
            Console.WriteLine("PC:{0}", PC);
            Console.WriteLine("       OpCode: 0x{0:X}", OpCode);

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
            char[] smbsToTrim = new char[] { '[', ']' };
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

    }
}
