using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ChangeApplyerTests
    {
        private int _noOfUpserts;
        [SetUp]
        public void SetUp()
        {
            _noOfUpserts = 0;
        }

        [Test]
        public void ApplyChanges_Upserts_Ok()
        {
            Subscriber changeApplyer= new Subscriber();

            ChangePushPackage insertPackage = new ChangePushPackage
                                                  {
                                                      Source = "Source1",
                                                      SourceRootName = @"Data\Test",
                                                      ChangePushItems =
                                                          new List<ChangePushItem>
                                                              {
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F12.xml",
                                                                          OperationType = OperationType.Insert
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F13\F13.xml",
                                                                          OperationType = OperationType.Insert
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F13\F13.xml",
                                                                          OperationType = OperationType.Update
                                                                      }

                                                              }
                                                  };
            changeApplyer.Initialize(@"Data\Output\Test");
            Assert.True(changeApplyer.ApplyChangePackage(insertPackage,  fakeMethod));
            Assert.AreEqual(3,_noOfUpserts);
        }

        private bool fakeMethod(string arg1, string arg2)
        {
            _noOfUpserts++;
            return true;
        }

        [Test]
        public void ApplyChanges_Upserts_SomeFail()
        {
            Subscriber changeApplyer = new Subscriber();

            ChangePushPackage insertPackage = new ChangePushPackage
            {
                Source = "Source1",
                SourceRootName = @"Data\Test",
                ChangePushItems =
                    new List<ChangePushItem>
                                                              {
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F11.xml",
                                                                          OperationType = OperationType.Insert
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F13\F12.xml",
                                                                          OperationType = OperationType.Insert
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F13\F13.xml",
                                                                          OperationType = OperationType.Update
                                                                      }

                                                              }
            };
            changeApplyer.Initialize(@"Data\Output\Test");
            Assert.False(changeApplyer.ApplyChangePackage(insertPackage,  fakeMethodFail1));
            Assert.AreEqual(3, _noOfUpserts);
        }

        private bool fakeMethodFail1(string arg1, string arg2)
        {
            _noOfUpserts++;
            return (arg1 != @"Data\Test\F1\F13\F12.xml");
        }

        [Test]
        public void ApplyChanges_Deletes_Ok()
        {
            Subscriber changeApplyer = new Subscriber();

            ChangePushPackage deletePackage = new ChangePushPackage
            {
                Source = "Source1",
                SourceRootName = @"Data\Test",
                ChangePushItems =
                    new List<ChangePushItem>
                                                              {
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F12.xml",
                                                                          OperationType = OperationType.Delete
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F121.xml",
                                                                          OperationType = OperationType.Delete
                                                                      }
                                                              }
            };
            changeApplyer.Initialize(@"Data\Output\Test");
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));
            Assert.True(changeApplyer.ApplyChangePackage(deletePackage,  fakeMethod));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));

        }

        [Test]
        public void ApplyChanges_Deletes_SomeNotExist()
        {
            Subscriber changeApplyer = new Subscriber();

            ChangePushPackage deletePackage = new ChangePushPackage
            {
                Source = "Source1",
                SourceRootName = @"Data\Test",
                ChangePushItems =
                    new List<ChangePushItem>
                                                              {
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F122.xml",
                                                                          OperationType = OperationType.Delete
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F12\F13.xml",
                                                                          OperationType = OperationType.Delete
                                                                      },
                                                                  new ChangePushItem
                                                                      {
                                                                          AbsolutePath = @"Data\Test\F1\F12\F121.xml",
                                                                          OperationType = OperationType.Delete
                                                                      }
                                                              }
            };
            changeApplyer.Initialize(@"Data\Output\Test");
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));
            Assert.False(changeApplyer.ApplyChangePackage(deletePackage,  fakeMethod));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));

        }
        
        [Test]
        public void ApplyChanges_NoChangePackage()
        {
            Subscriber changeApplyer = new Subscriber();
            Assert.False( changeApplyer.ApplyChangePackage(null, fakeMethod));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_Target_NotSent()
        {
            Subscriber changeApplyer = new Subscriber();
            changeApplyer.Initialize(string.Empty);
        }

    }
}
