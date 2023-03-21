using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; // Para thread e sleep
using System.Threading.Tasks;

namespace ProgramacaoParalela
{
    class Program
    {
        static Thread t1, t2;
        static bool letsFinish;
        static int idThread;
        static object objLock; // Associado ao lock() para resolver problema de concorrência.
        static Mutex objMutex; // Outra forma de resolver problema de concorrência.

        static void ThreadT1Routine()
        {
            try // Tratamento da exceção para o caso de abortagem da Thread
            {
                while (letsFinish == false)
                {
                    /*
                    lock (objLock) // Trava. Caos controlado. Uma das formas de resolver problema de concorrência.
                    {
                        idThread = 1; // Induz erro de concorrência quando não há lock
                        Console.WriteLine("Thread * 1 * -> +1s. Nº da Thread: " + idThread);
                        Thread.Sleep(1000);
                    }
                    */
                    objMutex.WaitOne(); // Trava que evita erro de concorrência.
                    idThread = 1; // Induz erro de concorrência quando não há lock/trava
                    Console.WriteLine("Thread * 1 * -> +1s. Nº da Thread: " + idThread);
                    Thread.Sleep(1000);
                    objMutex.ReleaseMutex(); // Destrava o Mutex.
                }

            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread * 1 * Exceção: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Thread * 1 * Finalizada");

            }
        }

        static void ThreadT2Routine()
        {
            try // Tratamento da exceção para o caso de abortagem da Thread
            {
                while (letsFinish == false)
                {
                    /*
                    lock (objLock) // Trava. Caos controlado. Uma das formas de resolver problema de concorrência.
                    {
                        idThread = 2; // Induz erro de concorrência quando não há lock
                        Console.WriteLine("Thread ** 2 ** -> +1s. Nº da Thread: " + idThread);
                        Thread.Sleep(1000);
                    }
                    */
                    objMutex.WaitOne(); // Trava que evita erro de concorrência.
                    idThread = 2; // Induz erro de concorrência quando não há lock/trava
                    Console.WriteLine("Thread ** 2 ** -> +1s. Nº da Thread: " + idThread);
                    Thread.Sleep(1000);
                    objMutex.ReleaseMutex(); // Destrava o Mutex.
                }
                
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread ** 2 ** Exceção: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Thread ** 2 ** Finalizada");
            }
        }

        static void Main(string[] args)
        {
            /* Usando o Thread.Sleep para gerar um delay:
             * 
            for(int i = 1; i <= 5; i++)
            {
                Thread.Sleep(1000); // Em milisegundos
                Console.WriteLine("Passou "+ i + " segundo(s)" );
            }
            Console.WriteLine("Finalizado");
            Console.ReadKey();
            *
            */

            objLock = new object(); // Instância objeto a ser usado como trava para as Threads

            objMutex = new Mutex(); // Outra forma de evitar erro de concorrência.

            letsFinish = false; // Flag que mantem o laço das Threads

            // Associa a Thread ao Método que deverá ser executado nela.
            t1 = new Thread(new ThreadStart(ThreadT1Routine));
            t2 = new Thread(new ThreadStart(ThreadT2Routine));


            /*
            - Estabelece prioridades de execução para as Threads.
            Essas prioridades serão mais relevantes/importantes,
            principalmente para o caso de tarefas que precisam ser 
            concluídas em um curto intervalo de tempo.
            - No exemplo atual, o processador ficará "chaveando" entre
            as Threads t1 e t2, e acabará executando mais frequentemente o
            Método associado à t1, pois ela tem a prioridade mais alta.
            */
            t1.Priority = ThreadPriority.Highest; // Alta prioridade, i.e, será executada com mais frequência.
            t2.Priority = ThreadPriority.BelowNormal; // Baixa prioridade, será executada com menos frequência.


            t1.Start(); // Dispara Método associado à t1
            t2.Start(); // Dispara Método associado à t2


            // O programa pode continuar enquanto os métodos de 
            // t1 e t2 rodam paralelamente.
            Console.ReadKey(); // Aguardando...

            // Vai fazer com que as Threads saiam do laço e parem de ser executadas.
            //letsFinish = true;


            /*
            - Forma alternativa de encerrar execução das Threads,
            onde elas são encerradas e não executarão mais nada.
            O programa não sairá do laço e executará o que está fora dele,
            a Thread simplesmente é abortada.Nem sempre é
            apropriado usar o Método.Abort().
            - .Abort() gera uma exceção, portanto, ao utilizá-lo,
            é recomendado implementar também um tratamento de
            exceção no método que é executado na Thread.

            t1.Abort();
            t2.Abort();
            */
            t1.Abort();
            t2.Abort();

            Thread.Sleep(1000);
            Console.WriteLine("Pressione qualquer tecla para finalizar");
            Console.ReadKey();
        }
    }
}
