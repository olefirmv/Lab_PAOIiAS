using System;


namespace Lab_PAOIiAS_1_new
{
    class Program
    {
        static int EAX = 0, EBX = 0, ECX = 0, EDX = 0;
        static int PC = 0;
        static int OpCode;
        static int[] cmem;

        static void Main(string[] args)
        {
            ArrInit();
            //cmds
            cmem[0] = 0x10003007; // load 1st index in cmem to ECX
            cmem[1] = 0x30000000;// L1
            cmem[2] = 0x11001003;// mov EAX cmem[ECX]
            cmem[3] = 0x20004001;// add two registers
            cmem[4] = 0x21003001;// Inc ECX 1
            cmem[5] = 0x31000000;// loop

            int tmpValue = cmem.Length;
            
            while (ECX != cmem.Length)
            {
                
                OpCode = DecodeOpCode(cmem[PC]);
                ShowPC(PC);
                           
                switch (OpCode)
                {
                    case 0x10:
                        // load
                        Load(ref ECX, 7);
                        break;
                    case 0x30:
                        //L1
                        Console.WriteLine("L1");
                        break;
                    case 0x11:
                        // mov
                        Mov(ref EAX, cmem[ECX]);
                        break;
                    case 0x20:
                        //add two registers
                        Add(ref EDX, EAX);
                        break;
                    case 0x21:
                        // Inc
                        Inc(ref ECX);
                        break;
                    case 0x31:
                        Console.WriteLine("Loop L1");
                        //loop
                        PC = 0;
                        break;
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

        static void Add(ref int reg1, int reg2)
        {
            reg1 += reg2;
        }
        
        static void Inc(ref int reg)
        {
            reg++;
        }

        static void Mov(ref int reg, int value)
        {
            reg = value;
        }

        static void Load (ref int reg, int firstNumberIndex)
        {
            reg = firstNumberIndex;
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
