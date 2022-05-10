using Core.Enums;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tradefact.Application.Helpers
{
    public class ShipmentStateMachine
    {
        public List<ShipmentAction> AvailableActions => this.AvailableShipmentAction().ToList();

        private IEnumerable<ShipmentAction> AvailableShipmentAction()
        {
            foreach (ShipmentAction action in Enum.GetValues(typeof(ShipmentAction)))
            {
                if (_shipmentStatusStateMachine.CanFire(action) && _shipmentStageStateMachine.CanFire(action))
                    yield return action;
            }
            yield break;
        }

        public ShipmentStatus CurrentStatus => _shipmentStatusStateMachine.State;
        public ShipmentStage CurrentStage => _shipmentStageStateMachine.State;

        private readonly StateMachine<ShipmentStatus, ShipmentAction> _shipmentStatusStateMachine;
        private readonly StateMachine<ShipmentStage, ShipmentAction> _shipmentStageStateMachine;

        public ShipmentStateMachine(ShipmentStatus shipmentStatus, ShipmentStage shipmentStage)
        {
            _shipmentStatusStateMachine = new StateMachine<ShipmentStatus, ShipmentAction>(shipmentStatus);
            _shipmentStageStateMachine = new StateMachine<ShipmentStage, ShipmentAction>(shipmentStage);

            ConfigureStateMachines();
        }

        private void ConfigureStateMachines()
        {
            _shipmentStatusStateMachine.Configure(ShipmentStatus.Active)
                .Permit(ShipmentAction.Suspend, ShipmentStatus.Suspended)
                .Permit(ShipmentAction.Terminate, ShipmentStatus.Terminated)
                .Ignore(ShipmentAction.AssignCollectionDate)
                .Ignore(ShipmentAction.RecordCollected)
                .Ignore(ShipmentAction.AssignTrackingInformation)
                .Permit(ShipmentAction.Complete, ShipmentStatus.Complete);

            _shipmentStatusStateMachine.Configure(ShipmentStatus.Suspended)
                .Permit(ShipmentAction.Reactivate, ShipmentStatus.Active)
                .Permit(ShipmentAction.Terminate, ShipmentStatus.Terminated);

            _shipmentStatusStateMachine.Configure(ShipmentStatus.Terminated)
                .Permit(ShipmentAction.Reactivate, ShipmentStatus.Active)
                .Permit(ShipmentAction.Archive, ShipmentStatus.Archived);

            _shipmentStatusStateMachine.Configure(ShipmentStatus.Terminated)
                .Permit(ShipmentAction.Reactivate, ShipmentStatus.Active);

            _shipmentStageStateMachine.Configure(ShipmentStage.Booked)
                .Ignore(ShipmentAction.Suspend)
                .Ignore(ShipmentAction.Terminate)
                .Permit(ShipmentAction.AssignCollectionDate, ShipmentStage.AwaitingCollection);

            _shipmentStageStateMachine.Configure(ShipmentStage.AwaitingCollection)
                .Ignore(ShipmentAction.Suspend)
                .Ignore(ShipmentAction.Terminate)
                .Permit(ShipmentAction.RecordCollected, ShipmentStage.InTransitToPort);

            _shipmentStageStateMachine.Configure(ShipmentStage.Shipping)
                .Ignore(ShipmentAction.Suspend)
                .Ignore(ShipmentAction.Terminate)
                .Permit(ShipmentAction.AssignTrackingInformation, ShipmentStage.DeparturePOLConfirmed);

            _shipmentStageStateMachine.Configure(ShipmentStage.InTransitToPort)
                .Ignore(ShipmentAction.Suspend)
                .Ignore(ShipmentAction.Terminate)
                .Permit(ShipmentAction.ConfirmDeparturePOL, ShipmentStage.Shipping)
                .Permit(ShipmentAction.Complete, ShipmentStage.Shipping);

            _shipmentStageStateMachine.Configure(ShipmentStage.DeparturePOLConfirmed)
                .Ignore(ShipmentAction.Suspend)
                .Ignore(ShipmentAction.Terminate)
                .Permit(ShipmentAction.Complete, ShipmentStage.Shipping);

        }

        void OnSetCollectionDate(DateTime collectionDate)
        {
            Console.WriteLine("Collection date set to: " + collectionDate + "!");
        }

        public void Suspend()
        {
            _shipmentStatusStateMachine.Fire(ShipmentAction.Suspend);
        }

        public void Terminate()
        {
            _shipmentStatusStateMachine.Fire(ShipmentAction.Terminate);
        }

        public void Reactivate()
        {
            _shipmentStatusStateMachine.Fire(ShipmentAction.Reactivate);
        }

        public void AssignCollectionDate()
        {
            _shipmentStageStateMachine.Fire(ShipmentAction.AssignCollectionDate);
        }

        public void MarkCollected()
        {
            _shipmentStageStateMachine.Fire(ShipmentAction.RecordCollected | ShipmentAction.RecordInTransitToPort);
        }

        public void MarkIntransit()
        {
            _shipmentStageStateMachine.Fire(ShipmentAction.ConfirmDeparturePOL);
        }

        //public void M()
        //{
        //    _shipmentStageStateMachine.Fire(ShipmentAction.ConfirmDeparturePOL);
        //}


        public bool CanSuspend => _shipmentStatusStateMachine.CanFire(ShipmentAction.Suspend);
        public bool CanReactivate => _shipmentStatusStateMachine.CanFire(ShipmentAction.Reactivate);
        public bool CanTerminate => _shipmentStatusStateMachine.CanFire(ShipmentAction.Terminate);
        public bool CanComplete => _shipmentStatusStateMachine.CanFire(ShipmentAction.Complete);
        public bool CanAssignCollectionDate => _shipmentStageStateMachine.CanFire(ShipmentAction.AssignCollectionDate);
        public bool CanMarkCollected => _shipmentStageStateMachine.CanFire(ShipmentAction.RecordCollected);
        public bool CanAddTrackingInformation => _shipmentStageStateMachine.CanFire(ShipmentAction.AssignTrackingInformation);
        public bool CanRecordInTransit => _shipmentStageStateMachine.CanFire(ShipmentAction.ConfirmDeparturePOL);

    }
}
