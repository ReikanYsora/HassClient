﻿using HassClient.Core.Tests;
using HassClient.Models;
using HassClient.Serialization;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(InputBooleanApiTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(InputBooleanApiTests) + "WithRealServer")]
    public class InputBooleanApiTests : BaseHassWSApiTest
    {
        private InputBoolean testInputBoolean;

        public InputBooleanApiTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        [OneTimeSetUp]
        [Test, Order(1)]
        public async Task CreateInputBoolean()
        {
            if (this.testInputBoolean == null)
            {
                this.testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:fan", true);
                var result = await this.hassWSApi.CreateInputBooleanAsync(this.testInputBoolean);

                Assert.IsTrue(result, "SetUp failed");
                return;
            }

            Assert.NotNull(this.testInputBoolean);
            Assert.NotNull(this.testInputBoolean.UniqueId);
            Assert.NotNull(this.testInputBoolean.Name);
            Assert.IsFalse(this.testInputBoolean.HasPendingChanges);
            Assert.IsTrue(this.testInputBoolean.IsTracked);
        }

        [Test, Order(2)]
        public async Task GetInputBooleans()
        {
            var result = await this.hassWSApi.GetInputBooleansAsync();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsTrue(result.Contains(this.testInputBoolean));
            Assert.IsTrue(result.Any(x => x.Name != null));
            Assert.IsTrue(result.Any(x => x.Initial == true));
            Assert.IsTrue(result.Any(x => x.Icon != null));
        }

        [Test, Order(3)]
        public async Task UpdateInputBooleanName()
        {
            this.testInputBoolean.Name = $"{nameof(InputBooleanApiTests)}_{DateTime.Now.Ticks}";
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [Test, Order(4)]
        public async Task UpdateInputBooleanInitial()
        {
            this.testInputBoolean.Initial = false;
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [Test, Order(5)]
        public async Task UpdateInputBooleanIcon()
        {
            this.testInputBoolean.Icon = $"mdi:lightbulb";
            var result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean);

            Assert.IsTrue(result);
        }

        [Test, Order(6)]
        public async Task UpdateWithForce()
        {
            var initialName = this.testInputBoolean.Name;
            var initialIcon = this.testInputBoolean.Icon;
            var initialInitial = this.testInputBoolean.Initial;
            var clonedEntry = this.testInputBoolean.Clone();
            clonedEntry.Name = $"{initialName}_cloned";
            clonedEntry.Icon = $"{initialIcon}_cloned";
            clonedEntry.Initial = !initialInitial;
            var result = await this.hassWSApi.UpdateInputBooleanAsync(clonedEntry);
            Assert.IsTrue(result, "SetUp failed");
            Assert.False(this.testInputBoolean.HasPendingChanges, "SetUp failed");

            result = await this.hassWSApi.UpdateInputBooleanAsync(this.testInputBoolean, forceUpdate: true);
            Assert.IsTrue(result);
            Assert.AreEqual(initialName, this.testInputBoolean.Name);
            Assert.AreEqual(initialIcon, this.testInputBoolean.Icon);
            Assert.AreEqual(initialInitial, this.testInputBoolean.Initial);
        }

        [OneTimeTearDown]
        [Test, Order(7)]
        public async Task DeleteInputBoolean()
        {
            if (this.testInputBoolean == null)
            {
                return;
            }

            var result = await this.hassWSApi.DeleteInputBooleanAsync(this.testInputBoolean);
            var deletedInputBoolean = this.testInputBoolean;
            this.testInputBoolean = null;

            Assert.IsTrue(result);
            Assert.IsFalse(deletedInputBoolean.IsTracked);
        }
    }
}
