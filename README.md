# TaskManagement

Instruction how to run the project

1. Applying Migrations. Check the connection string in appsettings.json. If it is ok - move on, otherwise - update it. Set as startup project 
   "TaskManagement.Api" and choose a default project "TaskManagement.Services" in Package manager console. Then execute command "Update-Database" 
   to roll migrations for database.

2. Configure rabbitmq on local computer. Instruction for windows => https://www.rabbitmq.com/install-windows.html .Check settings "RabbitMqOptions" 
   in appsettings.json. if the settings are not ok => set your own.
   
3. After that you can run TaskManagement.Api and test locally.