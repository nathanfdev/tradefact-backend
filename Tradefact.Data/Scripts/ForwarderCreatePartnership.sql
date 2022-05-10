
declare @client UNIQUEIDENTIFIER = '5f0a659b-d628-4bc3-a23d-18734ec94575',
        @provider UNIQUEIDENTIFIER = '4cd1bb58-9e5a-49fa-bb0f-22a365221a15'

select OT.Name, O.Id, O.[Name], O.*
from [dbo].[Organisations] O 
    INNER JOIN [dbo].OrganisationType OT ON OT.OrganisationTypeId = O.OrganisationTypeId
    where O.Id = @client

select OT.Name, O.Id, O.[Name] 
from [dbo].[Organisations] O 
    INNER JOIN [dbo].OrganisationType OT ON OT.OrganisationTypeId = O.OrganisationTypeId
    where O.Id = @provider

 BEGIN TRAN

    INSERT INTO [dbo].Partnerships (

        Id, ProviderId, ClientId, Active, CreatedByUser, CreationDateInternal, LastChangeUser, LastModifiedOnInternal, PartnershipTypeId
    )
    SELECT NEWID(), @provider, ClientId, Active, CreatedByUser, CreationDateInternal, LastChangeUser, LastModifiedOnInternal, PartnershipTypeId
    FROM [dbo].Partnerships P WHERE P.Id = '381f4f53-17bd-46b7-43d5-08d8e2f55ea8'

    SELECT top 10 P.Name, C.Name, PTY.Name, PT.* 
        FROM [dbo].[Partnerships] PT
            INNER JOIN [dbo].[Organisations] P ON P.Id = PT.ProviderId
            INNER JOIN [dbo].[Organisations] C ON C.Id = PT.ClientId
        LEFT JOIN PartnershipTypes PTY ON PTY.PartnershipTypeId = PT.PartnershipTypeId
    WHERE PT.ClientId = @client

ROLLBACK TRAN

select * from [dbo].[AspNetUsers] O where o.Email like '%up.ac.pa%'

select top 10 * from [dbo].[AspNetUserInvitations] O order by O.LastModifiedOnInternal desc