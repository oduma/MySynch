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
            ChangeApplyer changeApplyer= new ChangeApplyer();

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
            Assert.True(changeApplyer.ApplyChangePackage(insertPackage, @"Data\Output\Test", fakeMethod));
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
            ChangeApplyer changeApplyer = new ChangeApplyer();

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
            Assert.False(changeApplyer.ApplyChangePackage(insertPackage, @"Data\Output\Test", fakeMethodFail1));
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
            ChangeApplyer changeApplyer = new ChangeApplyer();

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
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));
            Assert.True(changeApplyer.ApplyChangePackage(deletePackage, @"Data\Output\Test", fakeMethod));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));

        }

        [Test]
        public void ApplyChanges_Deletes_SomeNotExist()
        {
            ChangeApplyer changeApplyer = new ChangeApplyer();

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
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));
            Assert.False(changeApplyer.ApplyChangePackage(deletePackage, @"Data\Output\Test", fakeMethod));
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
            Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));

        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyChanges_NoChangePackage()
        {
            ChangeApplyer changeApplyer = new ChangeApplyer();
            changeApplyer.ApplyChangePackage(null, "some folder", fakeMethod);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyChanges_Target_NotSent()
        {
            ChangeApplyer changeApplyer = new ChangeApplyer();
            changeApplyer.ApplyChangePackage(new ChangePushPackage(), string.Empty, fakeMethod);
        }

    }
}
