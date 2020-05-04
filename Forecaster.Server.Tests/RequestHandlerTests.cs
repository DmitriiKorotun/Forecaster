using System;
using System.IO;
using Forecaster.Net;
using Forecaster.Net.Requests;
using Forecaster.Server.Network;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forecaster.Server.Tests
{
    [TestClass]
    public class RequestHandlerTests
    {
        [TestMethod]
        public void RestoreRequest_InitialRequest_SameRequestExpected()
        {
            // arrange
            Request request = new HandshakeRequest(HandshakeRequest.RequestType.TransferFile);

            byte[] requestBytes = new RequestManager().CreateByteRequest(request);

            // act
            Request restoredRequest = RequestHandler.RestoreRequest<HandshakeRequest>(requestBytes);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(request, restoredRequest);

            Assert.IsTrue(comparsionResult.AreEqual);
        }

        [TestMethod]
        public void RestoreFileTransfer_InitialRequest_SameRequestExpected()
        {
            // arrange
            byte[] fileBytes = File.ReadAllBytes("fortests/XTSE-AAB.csv");
            
            FileTransferRequest request = new FileTransferRequest(fileBytes);

            byte[] requestBytes = new RequestManager().CreateByteRequest<FileTransferRequest>(request);

            // act
            Request restoredRequest = RequestHandler.RestoreRequest<FileTransferRequest>(requestBytes);

            // assert
            CompareLogic compareLogic = new CompareLogic();

            var comparsionResult = compareLogic.Compare(request, restoredRequest);

            Assert.IsTrue(comparsionResult.AreEqual);
        }
    }
}
