2023-06-20 12:02AM
Fix issue where the DR side does not have project code

2023-03-13 10:17PM
For TK Bakery Add HQ Code as ProjNo for Autocount only for FINANCIALPURCHASERECEIPTS
'FINANCIALPURCHASEVAR,FINANCIALPURCHASEINVOICES,FINANCIALSALESINVOICES';
For TK Bakery Remove Paymentment related posting
FINANCIALBANKDEPOSITS,FINANCIALAPPLYDEPOSIT,FINANCIALAPPLYPAYMENT,FINANCIALSALESFREIGHT
FINANCIALCUSTDEPOSIT,FINANCIALCUSTPAYMENT

2022-12-13 3:50PM
V2.0-B-15-R143-0.5
V2.0-B-15-R143-0.5Sql
V1.9-B4-R15-0.5

2022-02-22 5:00PM
V2.0-B-15-R143-0.4
V2.0-B-15-R143-0.4Sql
V1.9-B4-R15-0.4
There is no changes to the script. Only the webservice portion of the system is updated to post for GL with same document number with different date as different transactions
This was required for example in purchase receipt, when you unpost and change the date. Even though the document number is same the date is different


2021-10-14 10:05AM
V2.0-B-15-R143-0.3
V1.9-B4-R15-0.3
//Fix AP Tax Code missing

2021-10-14 1:45PM
V2.0-B-15-R143-0.2
Updated AP invoice and AP CN to use linescription


2021-07-07 11:45PM
Created separate installer for SqlAccounting version number will be
 V2.0-B15-R143-01Sql with Sql suffix

2021-01-17 12:19PM
Update dll for V2.0-B15-R143-01


2021-01-16 8:48PM
1)Change to version naming convention like below
V2.0-B7-R113-0.1
V1.9-B4-R15-0.1
First part like V1.9 or V2.0 is for DLL of Autocount. The number after the dash like B4-R15 is the release of the Autocount which is the actual dll compiled from the developer installed version. 
The last number after dash like 0.1 is for OstAutoCount or Script changes.
For new bundle it will contain 
a)Readme file
b)Release folder which will contain files to copy and paste to update OstAutoCount web service without Reinstalling
c)Full Installer
d)Script

2)Remove update to deposit amount
3)Updated logic for multiple lines of Customer Apply Payment when deleting, it will not cancel the Customer Payment in autcount if the Applied amount is not zero
4)Updated Script Ostendo AutoCount Link.txt to remove license dependency. Also change field for DepositAppliedAmount.
5)Updated Script Ostendo ScreenDataScriptCustomerPaymentsDelete to not allow deletion of Custom Payment with payment style of apply deposit if there are unposted Financial Apply depostis for the Payment.


2021-01-08 2:50AM
Update to 2.07 and update to 1.912
Update script to send in Payment No in description. Update to backend ws to check by Doc2 and also description. And will use this as determining factor whether a payment exist for applydeposit. 
If deposit amount is different then we will change the deposit amount before posting
If Payment amount is different then we will change the AR receipt amount to Payment amount
added execOstBizLinkWithDateCheck

2020-11-07 01:00AM
Update to 2.06
Change the login method to use MyDbSession instead of current Db Session as reported error is to do with login


2020-10-10 10:56AM
Update script for Ostbizlink as found the CN also need to match the Debtor control account by currency if not found like for sales invoice.
Added additional filter for retrieving for null batch number. This issue was found when user key in a future transactions like DN with future date.
Also note that for applying payments using CN user need to know that only fully applied items are knocked off and user must include screen data script (ScreenDataScriptCustomerPaymentsDelete.txt) to handle deletion of CN. User must also use (OST AC cnKnockOff) script to knock off also


2020-08-10 5:02PM
Update script for Ostbizlink because if debtor control account not found then we will look at forex debtor control account to post the credit side of journal.

2020-06-19 12:11PM
Update script to 2.03 and 2.04 distribution

2020-05-06 4:28PM
Update to 2.04
Update the link script for InsertOrUpdated column field
Update Autocount dll from 2.0.0 to 2.0.1

2020-05-03 8:20PM
Update to 2.03. 
Remove Take operator to not truncate the data being sent to ostautocount.
Fixed Multicurrency issue when debtor account in cost centre map is different from multicurrency debtor code settings.

2020-05-02 13:00PM
Update deploy to 2.0 This Only Works for Autocount 2.0. DLL might not work because of Restriction for copying dll from developement pc to actual Autocount installed location. Therefore you would need to replace from Actual Autocount 2.0 dll installed to replace those dll in ostAutocountws folder.

2020-02-26 7:36AM
Update to 1.19

2017-05-29 3:38PM
Added Installer for ostautocountws
Just Double click on installer to install. Make sure to check settings for AutoCount databse and login details
The Window Service name is called OstAutoCount
Add or remove program is called OstAutoCount version 1.16

2017-03-17 1:47AM
Added licensing component ostlicense.txt
Added License generator under licensegenerator.zip
Added Additional Table OSTDEF_OSTSETTINGS.dat and editview Ost Settings.dat
After importing edit view in ostendo you will need to update the settings
and also key in the product key in ostsettings editview. 
This product key is obtained from licensegenerator program. You specify company name and expiry date.
You must then export the license from licensegenerator program and copy this to ostautocountws main folder. The license must be named AutocountLink.lic



2017-01-13 12:31PM
Revert ApplyDeposit logic check for Debit Account Type as SDC. This setting is based on System Settings->Accounting Link->Other Accounting Solution and make sure to UNCHECK External Accounts Receivable

2017-01-05 6:35PM
For Customer Deposit with multicurrency will use the rate of 1/Exchangerate to update Autocount conversion rate
Use SOURCENUMBER as Document number for FinancialApplyPayment and FincancialApplyDeposit
Update logic for FinancialApplyDeposit. Refer excel file row 38 and 39
Update FinancialApplyDeposit to handle multiple invoices in apply payment


2017-01-03 7:01PM
For ApplyDeposit on select those payment APPLYORDISCOUNT = 'Apply'
Use defaultCurrency for CustomerPayment even if it's null to force Autocount to use local currency else it will use the Debtor code currency

2017-01-03 4:01PM
For Customer Apply Payment, add filter for apply payment ie APPLYORDISCOUNT = 'Apply' so that apply payment will work for multicurrency. If Not the gain/loss in the financial table clear out knock off of invoices.

2016-12-30 3:45PM
Fixed issue with AR-Refund in financialapplypayment
Take note that 
1)Ostendo version must be v221+ 
2)Accounting Link set to Other Accounting System so that Credit Account Type is SDC.

2016-12-29 6:57PM
Update ostcnKnockOff Script to handle null condition on deleted CN
Update ostautocount for Journal posting to check detail lines for Transaction Status instead of by Document number status.
Change knockoff for refund in api to KnockOff on Credit Account Type = "SDC" instead of debit account type.

2016-12-21 9:07AM
Must reapply additional field script (OSTDEF_CPDELETION)
 and delete payment script for screen data script(ScreenDataScriptCustomerPaymentsDelete) 
OstAutocount script now split into 3 script
1)ostSettings - set default settings in this script now
2)ostAutocount
3)ostCnKnockOff
Updated FINANCIALAPPLYPAYMENT for refund applied amount

2016-12-20 9:55AM
Update to use Autocount Debtor and Creditor code to autogenerate. Still need to specify in ostautocountlink script where to update the custcode and suppliercode, user must still supply Control Account Code
Added update Cheque No for Customer Payment
Added update Cheque No for Customer Deposit
Change Set Batch Number for ALL tables to use the same Batch Number for each process by day

2016-12-16 10:32PM
Retrieve FINANCIALPURCHASEVAR for transaction status <> 'Transaction Valid'
separate retrieval by ORDERTYPE is '' or not ''

2016-12-16 5:53PM
update to FINANCIALPURCHASERECEIPTS. Separate retrieval by ORDERTYPE Standard and '' 

2016-12-15 3:00PM
Added FINANCIALAPPLYPAYMENT
Added FINANCIALAPPLYDEPOSIT

2016-12-15 9:53AM
Fixed FINANCIALSALESINVOICE and FINANCIALPURCHASEINVOICE for local currency
Fixed Sales Cost unable to insert because of date
-updated - FINANCIALCUSTOMERDEPOSIT and FINANCIALCUSTPAYMENT

Note-For FINANCIALPURCHASERECEIPTS with foreign Currency you MUST set Cost Centres for
Foreign Purchase Receipts or else the CREDITCOST Centre will be null

2016-12-13 4:19PM
Remove Importing for GL accounts SDR and SCR
Remove Importing of Currency Code for local currency, settings in DefaultCurrency
Added posting for FINANCIALCUSTPAYMENT

Added posting for FINANCIALASSYISSUES
Added posting for FINANCIALASSYRECEIPTS
Added posting for FINANCIALASSYWIPVAR
Added posting for FINANCIALINVENTORY
Added posting for FINANCIALJOBISSUES
Added posting for FINANCIALJOBWIPVAR
Added posting for FINANCIALPURCHASERECEIPTS
Added posting for FINANCIALPURCHASEVAR
Added posting for FINANCIALSALESCOSTS
Added posting for FINANCIALSALESFREIGHT
Added posting for FINANCIALSALESISSUES


2016-12-08 11:49AM
Added Import AR By Multicurrency
Added Import AP By Multicurrency
Change CheckCreateDebtor to use FINANCIALCUSTOMERS and process by sysuniqueid for insert or update
Change CheckCreateCreditor to use FINANCIALSUPPLIERS and process by sysuniqueid for insert or update
-Note once Autocount imported debtor/creditor with currency and there is attached invoices you cant change the currency

2016-12-03 12:36AM
Change DocNo to BatchNo and use SupplierInvNo

2016-12-01 7:18PM
Added Cancel Document for Purchase Invoice
Added Cancel Document for Purchase Invoice Credit Note
2016-12-01 11:15AM
Added Purchase Invoices import excluding deleted Purchase Invoices
Added Purchase Invoice CN import excluding deleted Purchase Invoice CN