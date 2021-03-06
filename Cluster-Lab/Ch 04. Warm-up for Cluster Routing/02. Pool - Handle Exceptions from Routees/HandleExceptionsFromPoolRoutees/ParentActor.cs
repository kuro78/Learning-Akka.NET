﻿using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandleExceptionsFromPoolRoutees
{
    class ParentActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ParentActor())

                //
                // [[[ SupervisorStrategy 호출 이해 ]]]
                //
                // SupervisorStrategy 호출 우선순위 1.
                .WithSupervisorStrategy(new OneForOneStrategy(ex =>
                {
                    //
                    // _log.Info 함수를 호출할 수 없다.
                    // 
                    // Why?
                    // Props 함수가 static 이기 때문이다.
                    //
                    Context.GetLogger().Info($">>> OneForOneStrategy: { ex.Message}");

                    //
                    // [[[ Routing 장애 처리 전략 이해 ]]]
                    //
                    // Restart 시키면 Router(Child)가 재생성된다.
                    // 그 결과 모든 Routee($a, $b, ...) 역시 재생성된다. 
                    // 
                    // 부모       ParentActor
                    // Router     ChildActor       - Escalate
                    // Routee     $a
                    //
                    // By default, 
                    // pool routers use a custom strategy that only returns Escalate for all exceptions, 
                    // the router supervising the failing worker will then escalate to it's own parent, 
                    // if the parent of the router decides to restart the router, 
                    // all the pool workers will also be recreated as a result of this.
                    //

                    // 
                    // Pool 
                    //      Router을 재생성 시키기 대문에 
                    //      Routee 3개 $a, $b, $c 모두가 재생성된다.
                    //  vs.
                    // Group
                    //      예외가 발생된 특정 Routee 1개 $a만 재생성된다.
                    //
                    return Directive.Restart;
                }));
        }

        public ParentActor()
        {
            IActorRef childActor = Context.ActorOf(ChildActor.Props(), nameof(ChildActor));

            childActor.Tell(0);
            childActor.Tell(1);
            childActor.Tell(2);
            childActor.Tell(3);
        }

        //
        // SupervisorStrategy 호출 2. 우선순위이다.
        // WithSupervisorStrategy가 정의되어 있다면 SupervisorStrategy()는 호출되지 않는다.
        //
        protected override SupervisorStrategy SupervisorStrategy()
        {
            //
            // SupervisorStrategy()는 호출되지 않는다.
            //
            // Why?
            //  .WithSupervisorStrategy 보다 호출 우순가 낮기 때문이다.
            //
            return base.SupervisorStrategy();
        }
    }
}
