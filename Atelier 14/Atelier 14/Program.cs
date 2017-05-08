using System;

namespace AtelierXNA
{
   static class Program
   {
      static void Main(string[] args)
      {
         using (Atelier game = new Atelier())
         {
            game.Run();
         }
      }
   }
}

