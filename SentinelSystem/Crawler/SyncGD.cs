using System;
using System.Data;
using System.Threading.Tasks;
using Travox.Sentinel.Engine;
using Travox.Systems;
using System.Linq;
using System.Threading;

namespace Travox.Sentinel.Crawler
{
    public class SyncGD : Controller
    {
        ManualResetEventSlim[] mres;
        Int32 Idx = -1;
        String DBNameB2B;
        String DBNameB2C;

        private enum Business { B2B, B2C }
        private struct StateTask
        {
            public Int32 ID;
            public String DNS;
            public String DB;
            public Business Type;
        }


        public SyncGD()
        {
            base.SetEnabled = false;
            base.SetIntervel = new TimeSpan(0, 1, 0);
            String sql = "SELECT ISNULL(b2c_wh_code,'') b2c, ISNULL(b2b_wh_code,'') b2b FROM site_customer WHERE id = @id AND ISNULL(AutoInvoice_MBOS,'N') = 'Y'";
            foreach (DataRow Row in new DB("travox_global").GetTable(sql, new SQLCollection("@id", DbType.Int32, base.State.CompanyID)).Rows)
            {
                base.SetEnabled = true;
                DBNameB2B = Row["b2b"].ToString();
                DBNameB2C = Row["b2c"].ToString();
            }
        }

        public override void Start()
        {
            if (!MBOS.Null(DBNameB2C)) Idx++;
            if (!MBOS.Null(DBNameB2B)) Idx++;
            base.Start();
        }

        public override void Update()
        {
            String DNS = (App.DebugMode) ? "db9.ns.co.th" : "db2.ns.co.th";

            mres = new ManualResetEventSlim[Idx + 1];
            for (var i = 0; i < mres.Length; i++) mres[i] = new ManualResetEventSlim(false);
            if (!MBOS.Null(DBNameB2C)) Task.Factory.StartNew(GetPNR, new StateTask() { ID = Idx--, DB = DBNameB2C, DNS = DNS, Type = Business.B2C });
            if (!MBOS.Null(DBNameB2B)) Task.Factory.StartNew(GetPNR, new StateTask() { ID = Idx--, DB = DBNameB2B, DNS = DNS, Type = Business.B2B });

            WaitHandle.WaitAll((from x in mres select x.WaitHandle).ToArray());
            foreach (ManualResetEventSlim t in mres) t.Reset();

            base.Update();
        }

        public override void Stop()
        {
            base.Stop();
        }

        private void GetPNR(object item)
        {
            String StartMonth = DateTime.Now.AddDays(DateTime.Now.Day * -1).ToString("dd-MM-yyyy");
            StateTask State = (StateTask)item;
            SQLCollection param = new SQLCollection("@code", DbType.String, State.DB);
            param.ANDBetween("TICKET_DATE", "ticket_date", StartMonth, null, DbType.Date);
            
            switch(State.Type)
            {
                case Business.B2C: 
                    State.DB = new DB("travoxb2b_global", State.DNS, "travox").GetField("SELECT name FROM db_initial WHERE wholesale_code = @code ", param);
                    break;
                case Business.B2B: State.DB = "travoxb2b"; break;
            }

            DB GD = new DB(State.DB, State.DNS, "travox");
            //foreach (DataRow PNR in GD.GetTable(base.GetResource("GetPNR(view_header).sql"), param).Rows)
            //{
                //param.Add("@booking_info_id", DbType.String ,PNR["ref_id"].ToString());
                //String booking_pnr_id = GD.Execute("", WithBookingPNR(PNR));

                //param.Add("@booking_pnr_id", DbType.String, booking_pnr_id);

                 //   PR_view_header_Item.airline_code = PNR["airline_code"].ToString();
                 //   PR_view_header_Item.airline_name = PNR["airline_name"].ToString();
                 //   PR_view_header_Item.gds = PNR["gds"].ToString();
                 //   PR_view_header_Item.client_deadline = PNR["client_deadline"].ToString();
                 //   PR_view_header_Item.airline_deadline = PNR["airline_deadline"].ToString();
                 //   PR_view_header_Item.ticket_date = PNR["ticket_date"].ToString();
                 //   PR_view_header_Item.operate_by_code = PNR["operate_by_code"].ToString();
                 //   PR_view_header_Item.pnr = PNR["pnr"].ToString();
                 //   PR_view_header_Item.ad_cost = PNR["ad_cost")
                 //   PR_view_header_Item.ch_cost = PNR["ch_cost")
                 //   PR_view_header_Item.inf_cost = loopHeader
                 //   PR_view_header_Item.ad_price = PNR["ad_price")
                 //   PR_view_header_Item.ch_price = PNR["ch_price")
                 //   PR_view_header_Item.inf_price = loopHeader
                 //   PR_view_header_Item.ad_tax = PNR["ad_tax")
                 //   PR_view_header_Item.ch_tax = PNR["ch_tax")
                 //   PR_view_header_Item.inf_tax = loopHeader
                 //   PR_view_header_Item.currency = PNR["currency"].ToString();
                 //   PR_view_header_Item.credit_card_success = PNR["credit_card_success"].ToString();
                 //   PR_view_header_Item.service_charge = PNR["service_charge")
                 //   PR_view_header_Item.invoice_name = PNR["invoice_name"].ToString();
                 //   PR_view_header_Item.invoice_address = PNR["invoice_address"].ToString();
                 //   PR_view_header_Item.invoice_district = PNR["invoice_district"].ToString();
                 //   PR_view_header_Item.invoice_province = PNR["invoice_province"].ToString();
                 //   PR_view_header_Item.invoice_postcode = PNR["invoice_postcode"].ToString();
                 //   PR_view_header_Item.invoice_country = PNR["invoice_country"].ToString();
                 //   PR_view_header_Item.invoice_tel = PNR["invoice_tel"].ToString();
                 //   PR_view_header_Item.invoice_fax = PNR["invoice_fax"].ToString();
                 //   PR_view_header_Item.invoice_email = PNR["invoice_email"].ToString();
                 //   PR_view_header_Item.payment_type = PNR["payment_type"]
                 //   PR_view_header_Item.agent_name = PNR["agent_name"].ToString();
                 //   PR_view_header_Item.agent_address = PNR["agent_address"].ToString();
                 //   PR_view_header_Item.insurance_code = PNR["insurance_code"].ToString();
                 //   PR_view_header_Item.insurance_price = PNR["insurance_price"].ToString();
                 //   PR_view_header_Item.import_by = schema_name.ToString()
                 //   'PR_view_header.Add(PR_view_header_Item)
                 //   iReturn = dtReturn.Rows(0)("ref_id")
            //}

            mres[State.ID].Set();
        }

        private SQLCollection WithBookingPNR(DataRow Row)
        {
            SQLCollection param = new SQLCollection();
            param.Add("@airline_code", DbType.String, Row["airline_code"]);

            return param;
        }

    }
}
