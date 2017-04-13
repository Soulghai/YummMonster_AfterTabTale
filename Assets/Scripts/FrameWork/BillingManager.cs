using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;

	
public class BillingManager : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;

	// Use this for initialization
	void Start () {
		DefsGame.IAPs = this;
		RequestBillingProducts ();
	}

	private void OnEnable ()
	{
		// Register for callbacks
		Billing.DidFinishRequestForBillingProductsEvent	+= OnDidFinishProductsRequest;
		Billing.DidFinishProductPurchaseEvent	        += OnDidFinishTransaction;

		// For receiving restored transactions.
		Billing.DidFinishRestoringPurchasesEvent		+= OnDidFinishRestoringPurchases;

	}

	private void OnDisable ()
	{
		// Deregister for callbacks
		Billing.DidFinishRequestForBillingProductsEvent	-= OnDidFinishProductsRequest;
		Billing.DidFinishProductPurchaseEvent	        -= OnDidFinishTransaction;
		Billing.DidFinishRestoringPurchasesEvent		-= OnDidFinishRestoringPurchases;		
	}

	private bool IsAvailable ()
	{
//		return NPBinding.Billing.IsAvailable();
	    return false;
	}

	private bool CanMakePayments ()
	{
//		return NPBinding.Billing.CanMakePayments();
	    return false;
	}

	public void RequestBillingProducts ()
	{
//		NPBinding.Billing.RequestForBillingProducts(NPSettings.Billing.Products);

		// At this point you can display an activity indicator to inform user that task is in progress
	}

	private void OnDidFinishProductsRequest (BillingProduct[] _regProductsList, string _error)
	{
		// Hide activity indicator

		// Handle response
		if (_error != null)
		{        
			// Something went wrong
		}
		else 
		{  
			// Inject code to display received products
			foreach (BillingProduct _product in _regProductsList) {
				D.Log("Product Identifier = "         + _product.ProductIdentifier);
				D.Log("Product Description = "        + _product.Description);
			}
		}
	}

	public void BuyItem (BillingProduct _product)
	{
//		//if (NPBinding.Billing.IsProductPurchased(_product.ProductIdentifier))
//		if (NPBinding.Billing.IsProductPurchased(_product))
//		{
//			// Show alert message that item is already purchased
//
//			return;
//		}
//
//		// Call method to make purchase
//		NPBinding.Billing.BuyProduct(_product);

		// At this point you can display an activity indicator to inform user that task is in progress
	}

	public void BuyTier1()
	{
		// Buy the consumable product using its general identifier. Expect a response either 
		// through ProcessPurchase or OnPurchaseFailed asynchronously.
		FlurryEventsManager.dontSendLengthtEvent = true;
		BuyItem(NPSettings.Billing.Products[0]);
		FlurryEventsManager.SendEvent ("iap_clicked_<" + NPSettings.Billing.Products [0].ProductIdentifier + ">", DefsGame.screenCoins.prevScreenName);
	}

	public void BuyTier2()
	{
		// Buy the consumable product using its general identifier. Expect a response either 
		// through ProcessPurchase or OnPurchaseFailed asynchronously.
		FlurryEventsManager.dontSendLengthtEvent = true;
		BuyItem(NPSettings.Billing.Products[1]);
		FlurryEventsManager.SendEvent ("iap_clicked_<" + NPSettings.Billing.Products[1].ProductIdentifier + ">", DefsGame.screenCoins.prevScreenName);
	}

	public void BuyNoAds()
	{
		// Buy the non-consumable product using its general identifier. Expect a response either 
		// through ProcessPurchase or OnPurchaseFailed asynchronously.
		FlurryEventsManager.dontSendLengthtEvent = true;
		BuyItem(NPSettings.Billing.Products[2]);
		FlurryEventsManager.SendEvent ("iap_clicked_<" + NPSettings.Billing.Products[2].ProductIdentifier + ">", DefsGame.screenCoins.prevScreenName);
	}

	private void OnDidFinishTransaction (BillingTransaction _transaction)
	{
		FlurryEventsManager.dontSendLengthtEvent = true;

		Debug.Log ("OnDidFinishTransaction()");
		if (_transaction != null)
		{
			if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS)
			{
				if (_transaction.TransactionState == eBillingTransactionState.PURCHASED)
				{
					// Your code to handle purchased products
					if (_transaction.ProductIdentifier == NPSettings.Billing.Products [0].ProductIdentifier) {
						GameEvents.Send (OnAddCoinsVisual, 200);
						D.Log ("OnDidFinishTransaction() - 200 coins (bought)");

					} else if (_transaction.ProductIdentifier == NPSettings.Billing.Products [1].ProductIdentifier) {
							GameEvents.Send (OnAddCoinsVisual, 1000);
							D.Log ("OnDidFinishTransaction() - 1000 coins (bought)");
					} else if (_transaction.ProductIdentifier == NPSettings.Billing.Products [2].ProductIdentifier) {
						PublishingService.Instance.DisableAdsPermanently();

						DefsGame.noAds = 1;
						PlayerPrefs.SetInt ("noAds", DefsGame.noAds);
						D.Log ("OnDidFinishTransaction() - NoAds (bought)");
					} 

					FlurryEventsManager.SendEvent ("iap_completed_<" + _transaction.ProductIdentifier + ">", DefsGame.screenCoins.prevScreenName);

					BillingProduct product = NPBinding.Billing.GetStoreProduct(_transaction.ProductIdentifier);
					if (product != null)
						PublishingService.Instance.ReportPurchase(product.Price.ToString(), product.CurrencyCode);

					return;
				} else {
					NPBinding.UI.ShowAlertDialogWithSingleButton("Purchase failed", "", "Ok", (string _buttonPressed) => {});
				}
			}
			NPBinding.UI.ShowAlertDialogWithSingleButton("Purchase failed", "", "Ok", (string _buttonPressed) => {});
			return;
		}

		NPBinding.UI.ShowAlertDialogWithSingleButton("Purchase failed", "Check your Internet connection or try later!", "Ok", (string _buttonPressed) => {});
	}

	//public void BtnRestoreIaps() {
	//	NPBinding.Billing.RestorePurchases ();
	//	Debug.Log("BtnRestoreIaps()");
	//}

	public void BtnRestoreIaps() {
//		Debug.Log("BtnRestoreIaps()");
//		FlurryEventsManager.dontSendLengthtEvent = true;
//		NPBinding.Billing.RestorePurchases ();
	}

	private void OnDidFinishRestoringPurchases (BillingTransaction[] _transactions, string _error)
	{
		FlurryEventsManager.dontSendLengthtEvent = true;

		Debug.Log(string.Format("Received restore purchases response. Error = ", _error));

		if (_transactions != null)
		{                
			Debug.Log("Count of transaction information received = "+_transactions.Length.ToString());

			foreach (BillingTransaction _currentTransaction in _transactions)
			{

				if (_currentTransaction.TransactionState == eBillingTransactionState.RESTORED) {
					//if (_currentTransaction.ProductIdentifier == NPSettings.Billing.Products [0].ProductIdentifier) {
						DefsGame.noAds = 1;
						PlayerPrefs.SetInt ("noAds", DefsGame.noAds);
						PublishingService.Instance.DisableAdsPermanently ();
					//} 
				}
				Debug.Log("Product Identifier = "         + _currentTransaction.ProductIdentifier);
				Debug.Log("Transaction State = "        + _currentTransaction.TransactionState.ToString());
				Debug.Log("Verification State = "        + _currentTransaction.VerificationState);
				Debug.Log("Transaction Date[UTC] = "    + _currentTransaction.TransactionDateUTC);
				Debug.Log("Transaction Date[Local] = "    + _currentTransaction.TransactionDateLocal);
				Debug.Log("Transaction Identifier = "    + _currentTransaction.TransactionIdentifier);
				Debug.Log("Transaction Receipt = "        + _currentTransaction.TransactionReceipt);
				Debug.Log("Error = "                    + _currentTransaction.Error);
			}

			return;
		}

		NPBinding.UI.ShowAlertDialogWithSingleButton("Restore purchase failed", "", "Ok", (string _buttonPressed) => {});
	}

}
