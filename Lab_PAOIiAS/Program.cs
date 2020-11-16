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
            int[] cmem = new int[11];

            //cmds
            cmem[0] = 0x10000005; // load 1st index in cmem to ECX
            cmem[1] = 0x11000003;// mov EAX [ECX]
            cmem[2] = 0x20004001;// add two registers
            cmem[3] = 0x21003001;// Add ECX 1
            cmem[4] = 0x30000000;// loop

            //values
            cmem[5] = 0x00000005;
            cmem[6] = 0x00000005; // 1 num
            cmem[7] = 0x00000003; // 2 num
            cmem[8] = 0x00000001; // 3 num
            cmem[9] = 0x00000006; // 4 num
            cmem[10] = 0x0000000A; // 5 num




            while(ECX !=11)
            {
                
                
                OpCode = DecodeOpCode(cmem[PC]);
                Console.WriteLine("OpCode: 0x{0:X}", OpCode);
                
                //команды 
                 switch (OpCode)
                 {

                    case 0x10:
                        // load value to ECX
                        Load(ref ECX, 6);
                        PC = PC + 1;
                        Console.WriteLine("             PC: {0}", PC);
                        ShowRegisterValues(EAX, EBX, ECX, EDX);
                        
                        break;
                    case 0x11:
                                            // mov EAX [ECX]
                               Load(ref EAX, cmem[ECX]);
                               PC = PC + 1;
                                            Console.WriteLine("             PC: {0}", PC);
                                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                                            break;
                      case 0x30:
                                            // loop
                            
                                            PC = PC + 1;
                                            Console.WriteLine("             PC: {0}", PC);
                                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                                            break;
                      case 0x20:
                                            // add two registers
                                            int regNumber1 = DefineReg1(cmem[PC]);// номер регистра1
                                            int regNumber2 = DefineReg2(cmem[PC]);// номер регистра2
                                            // sum of register values
                                            int sumResult = AddRegReg(GetRegisterValue(regNumber1, EAX, EBX, ECX, EDX),
                                                                 GetRegisterValue(regNumber2, EAX, EBX, ECX, EDX));
                                            if (regNumber1 == 1)
                                                EAX = sumResult;
                                            else if (regNumber1 == 2)
                                                EBX = sumResult;
                                            else if (regNumber1 == 3)
                                                ECX = sumResult;
                                            else if (regNumber1 == 4)
                                                EDX = sumResult;
                                            PC = PC + 1;
                                            Console.WriteLine("             PC: {0}",PC);
                                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                        break;

                      case 0x21:
                                            // Add ECX value
                                            AddRegVal(ref ECX, 1);
                                            PC = PC + 1;
                                            Console.WriteLine("             PC: {0}",PC);
                                            ShowRegisterValues(EAX, EBX, ECX, EDX);
                                            PC = 1;
                                            break;
                 }
                
            }
            Console.WriteLine("Hex Result: 0x{0:X8}", EDX);
            Console.WriteLine("Int Result: {0}", EDX & 4095);
            if ((EDX & 4095) == expectedResult)
                Console.WriteLine("Register value equals the expected result");
            
        }
        static void AddRegVal (ref int ECX, int value)
        {
            ECX = ECX + 1;
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
        static int AddRegReg(int reg1, int reg2)
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