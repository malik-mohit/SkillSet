namespace SkillSet_Api.Model
{
    public class paging
    {
        public int pagesize { get; set; }
        public int pagenumber { get; set; }

        public paging()
        {
            this.pagesize = 1;
            this.pagenumber = 5;
        }

        public paging(int psize, int pnumber)
        {
            this.pagesize = psize > 10 ? 10 : psize;
            this.pagenumber = pnumber < 1 ? 1 : pnumber;
        }
    }
}
