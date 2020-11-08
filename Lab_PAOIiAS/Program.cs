using System;
using System.Text;

namespace Lab_PAOIiAS
{
    class Program
    {
        static void Main(string[] args)
        {
            //регистры
            int EAX, EBX, ECX, EDX = 0;

            // счетчик команд, команд операций
            int PC, OpCode;

            //массив чисел для сложения
            int[] numbers = new int[] { 5, 3, 1, 6, 9};
            
            //оперативная память
            int[] cmem = new int[400];

            for (int i=0; i < cmem.Length; i++)
            {
                
                OpCode = DecodeOpCode(cmem[i]);
                //команды
                switch (OpCode)
                {
                    case 0x10:
                        
                    case 0x11:
                    case 0x12:
                    case 0x13:
                    default: 
                        break;
                }
            }           
        }

        static int DecodeOpCode (int commandNumber )
        {
            commandNumber = commandNumber >> 24;
            return commandNumber;
        }
    }
}
