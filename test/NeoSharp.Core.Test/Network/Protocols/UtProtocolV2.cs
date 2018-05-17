﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Protocols;
using NeoSharp.Core.Test.Builders;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network.Protocols
{
    [TestClass]
    public class UtProtocolV2 : TestBase
    {
        [TestMethod]
        public async Task SendReceiveMessage_VerAckMessageSended_VerAckMessageReveivedIsTheSameAsSended()
        {
            var sendedVerAckMessage = new VerAckMessage();
            Message receivedMessage;

            var testee = this.AutoMockContainer.Create<ProtocolV2>();
            using (var memoryStream = new MemoryStream())
            {
                await testee.SendMessageAsync(memoryStream, sendedVerAckMessage, CancellationToken.None);

                memoryStream.Seek(0, SeekOrigin.Begin);

                receivedMessage = await testee.ReceiveMessageAsync(memoryStream, CancellationToken.None);
            }

            receivedMessage
                .Should()
                .BeOfType<VerAckMessage>()
                .And
                .NotBeNull()
                .And
                .BeEquivalentTo(sendedVerAckMessage);
        }

        [TestMethod]
        public async Task SendReceiveMessage_ValidVersionMessageWithZeroLengthSended_ReceiveMessageUncompressedAndEqualToSended()
        {
            this.AutoMockContainer.Register<IBinaryConverter>(new BinaryConverter(typeof(VersionMessage).Assembly));

            var sendedVersionMessage = new VersionMessageBuilder()
                .WithLength(0)
                .Build();
            Message receivedMessage;

            var testee = this.AutoMockContainer.Create<ProtocolV2>();
            using (var memoryStream = new MemoryStream())
            {
                await testee.SendMessageAsync(memoryStream, sendedVersionMessage, CancellationToken.None);

                memoryStream.Seek(0, SeekOrigin.Begin);

                receivedMessage = await testee.ReceiveMessageAsync(memoryStream, CancellationToken.None);
            }

            receivedMessage
                .Should()
                .BeOfType<VersionMessage>()
                .And
                .NotBeNull()
                .And
                .Match<VersionMessage>(x => !x.Flags.HasFlag(MessageFlags.Compressed))
                .And
                .BeEquivalentTo(sendedVersionMessage);
        }

        [TestMethod]
        public async Task SendReceiveMessage_ValidVersionMessageWith200LengthSended_ReceiveMessageCompressedAndEqualToSended()
        {
            this.AutoMockContainer.Register<IBinaryConverter>(new BinaryConverter(typeof(VersionMessage).Assembly));

            var sendedVersionMessage = new VersionMessageBuilder()
                .WithLength(200)
                .Build();
            Message receivedMessage;

            var testee = this.AutoMockContainer.Create<ProtocolV2>();
            using (var memoryStream = new MemoryStream())
            {
                await testee.SendMessageAsync(memoryStream, sendedVersionMessage, CancellationToken.None);

                memoryStream.Seek(0, SeekOrigin.Begin);

                receivedMessage = await testee.ReceiveMessageAsync(memoryStream, CancellationToken.None);
            }

            receivedMessage
                .Should()
                .BeOfType<VersionMessage>()
                .And
                .NotBeNull()
                .And
                .Match<VersionMessage>(x => x.Flags.HasFlag(MessageFlags.Compressed))
                .And
                .BeEquivalentTo(sendedVersionMessage);
        }
    }
}