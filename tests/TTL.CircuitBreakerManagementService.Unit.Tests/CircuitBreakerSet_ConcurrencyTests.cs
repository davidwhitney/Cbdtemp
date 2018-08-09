using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    [TestFixture]
    internal class CircuitBreakerSet_ConcurrencyTests
    {
        [Test]
        [Category(TestCategory.LongRunning)]
        public void can_concurrently_add_circuits_burnin()
        {
            for (int i = 0; i < 1000; i++)
            {
                can_concurrently_add_circuits();
            }
        }

        [Test]
        [Category(TestCategory.LongRunning)]
        public void can_concurrently_update_circuitbreakers_burnin()
        {
            for (int i = 0; i < 1000; i++)
            {
                can_concurrently_update_circuitbreakers();
            }
        }


        [Test]
        public void can_concurrently_add_circuits()
        {
            var set = new CircuitBreakerSet();
            int counter = 0;
            using (ManualResetEvent waithandle = new ManualResetEvent(false))
            {
                for (int i = 0; i < 10000; i++)
                {
                    ThreadPool.QueueUserWorkItem(o =>
                        {
                            Helper.AddRandomCircuit(set);
                            Interlocked.Increment(ref counter);
                            if (counter == 10000)
                            {
// ReSharper disable AccessToDisposedClosure
                                waithandle.Set();
// ReSharper restore AccessToDisposedClosure
                            }
                        });
                }
                waithandle.WaitOne(10000);
                Assert.That(set.Circuits().Length, Is.EqualTo(10000));
            }
        }

        [Test]
        public void can_concurrently_update_circuitbreakers()
        {
            object monitor = new object();

            int numcircuits = 13;
            int[] maxSequenceNumber = new int[numcircuits];

            var set = new CircuitBreakerSet();
            int counter = 0;
            int updateCounter = 0;
            using (ManualResetEvent waithandle = new ManualResetEvent(false))
            {
                for (int i = 0; i < 10000; i++)
                {
                    ThreadPool.QueueUserWorkItem(o =>
                        {
                            int sequenceNumber;
                            string circuitId;

                            lock (monitor)
                            {
                                int currentCircuit = (counter%numcircuits);
                                circuitId = currentCircuit.ToString(CultureInfo.InvariantCulture);

                                sequenceNumber = counter;
                                maxSequenceNumber[currentCircuit] = sequenceNumber;
                                counter++;
                            }
                            Helper.UpdateSpecificCircuit(set, circuitId, sequenceNumber);

                            lock (monitor)
                            {
                                updateCounter++;  //  you need a counter for both sequence and updates. Otherwise a race emerges either between setting the waithandle too early or sending multiple updates with a counter of zero. 
                                if (updateCounter == 10000)     
                                {
// ReSharper disable AccessToDisposedClosure
                                    waithandle.Set();
// ReSharper restore AccessToDisposedClosure
                                }
                            }
                        });
                }
                waithandle.WaitOne(10000);
                Assert.That(set.Circuits().Length, Is.EqualTo(numcircuits));
                for (int i = 0; i < numcircuits; i++)
                {
                    Assert.That(set.TryGetCircuit(i.ToString(CultureInfo.InvariantCulture)).MessageSequenceNumber, Is.EqualTo(maxSequenceNumber[i]));
                }
            }
        }
    }

    internal class TestCategory
    {
        public const string LongRunning = "LongRunning";
    }
}