using System.Configuration;

namespace SimpleCqrs.NServiceBus.Eventing.Config
{
    public class DomainEventEndpointMapping : ConfigurationElement
    {
        private const string DomainEventsPropertyName = "DomainEvents";
        private const string QueueNamePropertyName = "QueueName";
        private const string MachineNamePropertyName = "MacnineName";

        [ConfigurationProperty(QueueNamePropertyName, IsRequired = true, IsKey = true)]
        public string QueueName
        {
            get { return (string)base[QueueNamePropertyName]; }
            set { base[QueueNamePropertyName] = value; }
        }

        [ConfigurationProperty(MachineNamePropertyName, IsRequired = true, IsKey = true)]
        public string MachineName
        {
            get { return (string)base[MachineNamePropertyName]; }
            set { base[MachineNamePropertyName] = value; }
        }

        [ConfigurationProperty(DomainEventsPropertyName, IsRequired = true, IsKey = true)]
        public string DomainEvents
        {
            get { return (string)base[DomainEventsPropertyName]; }
            set { base[DomainEventsPropertyName] = value; }
        }
    }
}