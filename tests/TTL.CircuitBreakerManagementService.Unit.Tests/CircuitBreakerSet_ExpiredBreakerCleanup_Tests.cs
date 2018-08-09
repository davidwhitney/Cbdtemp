using System;
using NUnit.Framework;
using Ttl.CircuitBreakerManagementService.Impl;

namespace Ttl.CircuitBreakerManagementService.Unit.Tests
{
    [TestFixture]
    class CircuitBreakerSet_ExpiredBreakerCleanup_Tests
    {
        private CircuitBreakerSet _set;        

        [SetUp]
        public void Setup()
        {
            _set = new CircuitBreakerSet();
        }

        [Test]
        public void cleaning_up_an_empty_set_does_nothing()
        {
            _set.RemoveExpiredCircuitBreakers(SystemTime.Now());

            Assert.That(_set.Circuits().Length, Is.EqualTo(0));
        }

        [Test]
        public void cleaning_up_a_set_with_DateTimeMinValue_does_nothing()
        {
            _set = new CircuitBreakerSet();

            Helper.AddRandomCircuit(_set);
            _set.RemoveExpiredCircuitBreakers(DateTime.MinValue);

            Assert.That(_set.Circuits().Length, Is.EqualTo(1));
        }

        [Test]
        public void cleaning_up_a_set_with_DateTimeMaxValue_wipes_set()
        {
            Helper.AddRandomCircuit(_set);
            _set.RemoveExpiredCircuitBreakers(DateTime.MaxValue);

            Assert.That(_set.Circuits().Length, Is.EqualTo(0));
        }

        [Test]
        public void an_entry_gets_wiped_when_expirydate_matches()
        {
            var id = Helper.AddRandomCircuit(_set);
            var state = _set.TryGetCircuit(id);
            _set.RemoveExpiredCircuitBreakers(state.LastModifiedDate);

            Assert.That(_set.Circuits().Length, Is.EqualTo(0));
        }

        [Test]
        public void all_entries_older_than_expirydate_get_wiped_leaving_newer_ones_untouched()
        {
            for (int i = 0; i < 13; i++)
            {
                var dt = SystemTime.Is(i);
                var c = Helper.AddRandomCircuit(_set);
                var cb = _set.TryGetCircuit(c);
                Assert.AreEqual(dt, cb.LastModifiedDate);
            }
            Assert.That(_set.Circuits().Length, Is.EqualTo(13));
            
            
            DateTime expiryDateTime = SystemTime.Is(10);
            _set.RemoveExpiredCircuitBreakers(expiryDateTime);
            
            Assert.That(_set.Circuits().Length, Is.EqualTo(2));
            foreach(var id in _set.Circuits())
            {
                var state = _set.TryGetCircuit(id);
                Assert.That(state.LastModifiedDate > expiryDateTime);
            }
        }
    }
}
