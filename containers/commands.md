`podman run -d --name rabbitmq-100 -p 5673:5672 -p 15673:15672 docker.io/library/rabbitmq:3-management bash -c "rabbitmq-plugins enable --offline rabbitmq_amqp1_0 rabbitmq_management && rabbitmq-server"`


`podman run -d --name rabbitmq-091 -p 5672:5672 -p 15672:15672 docker.io/library/rabbitmq:3-management`
