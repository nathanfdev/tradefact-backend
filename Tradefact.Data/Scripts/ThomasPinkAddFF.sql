SELECT O.Name,  P.* FROM [dbo].[Partnerships] P 
    INNER JOIN [dbo].[Organisations] O ON O.Id = P.ProviderId
WHERE P.ClientId = '5f0a659b-d628-4bc3-a23d-18734ec94575'

BEGIN TRAN

INSERT INTO [dbo].[Partnerships] (Id, ProviderId, ClientId, Active, CreatedByUser, CreationDateInternal, LastChangeUser, LastModifiedOnInternal, PartnershipTypeId)
SELECT NEWID(), 'f9140256-1f86-486a-abd8-5d9f69c0502a', ClientId, Active, CreatedByUser, CreationDateInternal, LastChangeUser, LastModifiedOnInternal, PartnershipTypeId
 FROM [dbo].[Partnerships] P 
WHERE P.Id = '381f4f53-17bd-46b7-43d5-08d8e2f55ea8'

ROLLBACK TRAN

