using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;
using MySynch.Core.Subscriber;
using NUnit.Framework;

namespace MySynch.Tests
{
    //[TestFixture]
    //public class ChangeApplyerTests
    //{

    //    [Test]
    //    public void ApplyChanges_Upserts_Ok()
    //    {
    //        Subscriber changeApplyer = new Subscriber();

    //        PublishPackageRequestResponse insertPackageRequestResponse = new PublishPackageRequestResponse
    //                                              {
    //                                                  Source = "Source1",
    //                                                  SourceRootName = @"Data\Test\",
    //                                                  ChangePushItems =
    //                                                      new List<ChangePushItem>
    //                                                          {
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F12.xml",
    //                                                                      OperationType = OperationType.Insert
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F13\F13.xml",
    //                                                                      OperationType = OperationType.Insert
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F13\F13.xml",
    //                                                                      OperationType = OperationType.Update
    //                                                                  }

    //                                                          }
    //                                              };
    //        changeApplyer.Initialize(@"Data\Output\Test\");
    //        var mockCopyStrategy = MockCopyStrategy();
    //        changeApplyer.InitiatedCopyStrategies = mockCopyStrategy;
    //        foreach(var changePushItem in insertPackageRequestResponse.ChangePushItems)
    //        {
    //            var response =
    //                changeApplyer.TryApplyChange(new ReceiveMessageRequest
    //                                                 {
    //                                                     PublisherMessage = changePushItem,
    //                                                     SourceRootName = insertPackageRequestResponse.SourceRootName
    //                                                 });
    //            Assert.True(response.Success);
    //            Assert.AreNotEqual(changePushItem.AbsolutePath,response.ChangePushItem.AbsolutePath);
    //        }
    //    }

    //    [Test]
    //    public void ApplyChanges_Upserts_SomeFail()
    //    {
    //        Subscriber changeApplyer = new Subscriber();

    //        PublishPackageRequestResponse insertPackageRequestResponse = new PublishPackageRequestResponse
    //        {
    //            Source = "Source1",
    //            SourceRootName = @"Data\Test\",
    //            ChangePushItems =
    //                new List<ChangePushItem>
    //                                                          {
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F11.xml",
    //                                                                      OperationType = OperationType.Insert
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F13\F12.xml",
    //                                                                      OperationType = OperationType.Insert
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F13\F13.xml",
    //                                                                      OperationType = OperationType.Update
    //                                                                  }

    //                                                          }
    //        };
    //        changeApplyer.Initialize(@"Data\Output\Test\");
    //        var mockCopyStrategy = MockCopyStrategy();
    //        changeApplyer.InitiatedCopyStrategies = mockCopyStrategy;
    //        foreach (var changePushItem in insertPackageRequestResponse.ChangePushItems)
    //        {
    //            var response =
    //                changeApplyer.TryApplyChange(new ReceiveMessageRequest
    //                {
    //                    PublisherMessage = changePushItem,
    //                    SourceRootName = insertPackageRequestResponse.SourceRootName
    //                });
    //            if (response.ChangePushItem.AbsolutePath == @"Data\Output\Test\F1\F13\F12.xml")
    //                Assert.False(response.Success);
    //            else
    //                Assert.True(response.Success);
    //            Assert.AreNotEqual(changePushItem.AbsolutePath, response.ChangePushItem.AbsolutePath);
    //        }
    //    }

    //    private ICopyStrategy MockCopyStrategy()
    //    {
    //        var mockCopyStrategy = new Mock<ICopyStrategy>();
    //        mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F12\F12.xml", @"Data\Output\Test\F1\F12\F12.xml")).Returns(true);
    //        mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F13\F13.xml", @"Data\Output\Test\F1\F13\F13.xml")).Returns(true);
    //        mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F13\F12.xml", @"Data\Output\Test\F1\F13\F12.xml")).Returns(false);
    //        mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F12\F11.xml", @"Data\Output\Test\F1\F12\F11.xml")).Returns(true);
    //        mockCopyStrategy.Setup(m => m.Copy(@"root folder\Item One", @"destination root folder\Item One")).Returns(true);
    //        mockCopyStrategy.Setup(m => m.Copy(@"root folder\ItemTwo", @"destination root folder\ItemTwo")).Returns(true);
    //        return mockCopyStrategy.Object;
    //    }

    //    [Test]
    //    public void ApplyChanges_Deletes_Ok()
    //    {
    //        Subscriber changeApplyer = new Subscriber();

    //        PublishPackageRequestResponse deletePackageRequestResponse = new PublishPackageRequestResponse
    //        {
    //            Source = "Source1",
    //            SourceRootName = @"Data\Test",
    //            ChangePushItems =
    //                new List<ChangePushItem>
    //                                                          {
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F12.xml",
    //                                                                      OperationType = OperationType.Delete
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F121.xml",
    //                                                                      OperationType = OperationType.Delete
    //                                                                  }
    //                                                          }
    //        };
    //        changeApplyer.Initialize(@"Data\Output\Test");
    //        Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
    //        Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));
    //        var mockCopyStrategy = MockCopyStrategy();
    //        changeApplyer.InitiatedCopyStrategies = mockCopyStrategy;
    //        foreach (var changePushItem in deletePackageRequestResponse.ChangePushItems)
    //        {
    //            var response =
    //                changeApplyer.TryApplyChange(new ReceiveMessageRequest
    //                {
    //                    PublisherMessage = changePushItem,
    //                    SourceRootName = deletePackageRequestResponse.SourceRootName
    //                });
    //            Assert.True(response.Success);
    //            Assert.AreNotEqual(changePushItem.AbsolutePath, response.ChangePushItem.AbsolutePath);
    //        }
    //        Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
    //        Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F121.xml"));

    //    }

    //    [Test]
    //    public void ApplyChanges_Deletes_SomeNotExist()
    //    {
    //        Subscriber changeApplyer = new Subscriber();

    //        PublishPackageRequestResponse deletePackageRequestResponse = new PublishPackageRequestResponse
    //        {
    //            Source = "Source1",
    //            SourceRootName = @"Data\Test",
    //            ChangePushItems =
    //                new List<ChangePushItem>
    //                                                          {
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F122.xml",
    //                                                                      OperationType = OperationType.Delete
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F12\F13.xml",
    //                                                                      OperationType = OperationType.Delete
    //                                                                  },
    //                                                              new ChangePushItem
    //                                                                  {
    //                                                                      AbsolutePath = @"Data\Test\F1\F12\F121.xml",
    //                                                                      OperationType = OperationType.Delete
    //                                                                  }
    //                                                          }
    //        };
    //        changeApplyer.Initialize(@"Data\Output\Test");
    //        Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
    //        Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));
    //        var mockCopyStrategy = MockCopyStrategy();
    //        changeApplyer.InitiatedCopyStrategies = mockCopyStrategy;
    //        foreach (var changePushItem in deletePackageRequestResponse.ChangePushItems)
    //        {
    //            var response =
    //                changeApplyer.TryApplyChange(new ReceiveMessageRequest
    //                {
    //                    PublisherMessage = changePushItem,
    //                    SourceRootName = deletePackageRequestResponse.SourceRootName
    //                });
    //            Assert.True(response.Success);
    //            Assert.AreNotEqual(changePushItem.AbsolutePath, response.ChangePushItem.AbsolutePath);
    //        }
    //        Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F122.xml"));
    //        Assert.False(File.Exists(@"Data\Output\Test\F12\F13.xml"));

    //    }

    //    [Test]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public void Initialize_Target_NotSent()
    //    {
    //        Subscriber changeApplyer = new Subscriber();
    //        changeApplyer.Initialize(string.Empty);
    //    }

    //}
}
