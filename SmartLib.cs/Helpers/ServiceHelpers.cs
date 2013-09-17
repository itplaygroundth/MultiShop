using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SmartLib.Helpers
{
    public partial class ServiceHelpers
    {
        private static ServiceHost host = null;

        private static int count;
        public static int Count
        {
            get
            {
                count += 1;
                return count;
            }
        }
        public static void StartService()
        {
            
     

            //host  = new ServiceHost(typeof(SmartWCF.EventService));

            //host.AddServiceEndpoint(typeof(SmartWCF.IEventSystem), new NetTcpBinding(), new Uri("net.tcp://localhost:9999/EventService/"));
            ////host.AddServiceEndpoint(typeof(EventService), new WSDualHttpBinding(), "http://localhost:9000/EventService");
            //ServiceBehaviorAttribute attribute = (ServiceBehaviorAttribute)host.Description.Behaviors[typeof(ServiceBehaviorAttribute)];
            //attribute.Name = "ExposeMexAndThrottleBehavior";
            //attribute.ConcurrencyMode = ConcurrencyMode.Multiple;
            ////attribute.InstanceContextMode = InstanceContextMode.Single;

            //ServiceThrottlingBehavior throttling = new ServiceThrottlingBehavior();
            //throttling.MaxConcurrentCalls = 3;
            //throttling.MaxConcurrentInstances = 100;
            //throttling.MaxConcurrentSessions = 100;
            //host.Description.Behaviors.Add(throttling);



            //host.Closing += new EventHandler(host_Closing);
            //host.Open();
            ////Console.WriteLine("Service Started");

            //System.Threading.Timer timer = new System.Threading.Timer(delegate(object state)
            //{
            //    string message = "Message " + Count.ToString();
            //    try
            //    {
            //        if (SmartWCF.EventService.MessageReceived != null)
            //        {
            //            //Console.WriteLine(SmartWCF.EventService.MessageReceived.GetInvocationList().Length.ToString() + " Subscribers");
            //            SmartWCF.EventService.SendMessage(message);
            //            //Console.WriteLine("Sent: " + message);
            //        }
            //        else
            //        {
            //            //Console.WriteLine("0 Subscribers");
            //            //Console.WriteLine("Skipped: " + message);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.ToString());
            //    }
            //}, null, 5000, 5000);

           // Console.ReadKey(true);
           // host.Close();
        }

        static void host_Closing(object sender, EventArgs e)
        {
            try
            {
                SmartWCF.EventService.NotifyServiceStop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void StopService()
        {
        	if(host!=null)
            {
            	if(host.State==CommunicationState.Opened)
                {
                    host.Close();

                }
            }
        }
    }
}
