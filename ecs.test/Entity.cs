using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asshcii.ecs.test {

    class EntityImpl : asshcii.ecs.Entity {
        public EntityImpl(string name) : base(name) { }
    }

    class TestComponent : asshcii.ecs.IComponent{ }

    [TestClass]
    public class Entity {
        [TestMethod]
        public void TestName(){
            var testName = "Test";
            var entity = new EntityImpl(testName);
            Assert.AreEqual(entity.Name, testName);

            entity = new EntityImpl("Blah");
            Assert.AreNotEqual(entity.Name, testName);
        }

        [TestMethod]
        public void TestComponents(){
            var entity = new EntityImpl("Test");

            try {
                entity.GetComponent<TestComponent>();
                Assert.Fail("Entity should throw if the component doesn't exist");
            } catch(System.InvalidOperationException) {
                // Succeeds
            }
            
            entity.AddComponent(new TestComponent());

            Assert.IsNotNull(entity.GetComponent<TestComponent>());
        }
    }
}
