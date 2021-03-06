﻿using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeedNode1
{
    public class FooActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new FooActor());
        }

        public FooActor()
        {
            Receive<string>(_ => Handle(_));

            _log.Info($">>> Foo Address : {Self.Path.ToStringWithAddress()}");

        }

        private void Handle(string msg)
        {
            _log.Info($">>> Message : {msg}, Sender: {Sender}");

            IActorRef nonSeedNode3Foo = Sender;
            nonSeedNode3Foo.Tell("Callback Hello");
        }
    }
}
