using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpUI;

namespace AlgorithmTest
{
    class SJF
    {
        private List<ProcessData> data;
        private List<ProcessData> estimate_data;

        private List<int> delay_data;
        private List<int> return_data;
        private List<bool> end_job_flag;

        public SJF( List<ProcessData> pre_data )
        {
            data = pre_data;

            estimate_data = new List<ProcessData>();
            delay_data = new List<int>();
            return_data = new List<int>();

            data = Common.Sort_Initialize( data );
        }

        public List<ProcessData> working()
        {
            if ( estimate_data.Count == 0 )
            {
                List<ProcessData> ready_queue = new List<ProcessData>();
                end_job_flag = new List<bool>();

                int limit = data.Sum( item => Convert.ToInt32( item.arrived_time ) ) + data.Sum( item => Convert.ToInt32( item.service_time ) );

                init();

                for ( int time = 0; time < limit; )
                {
                    for ( int i = 0; i < data.Count; i++ )
                    {
                        if ( Convert.ToInt32( data[ i ].arrived_time ) <= time &&
                            !end_job_flag[ Convert.ToInt32( data[ i ].no ) - 1 ] )
                        {
                            ready_queue.Add( data[ i ] );
                            // 작업 수행 확인
                            end_job_flag[ Convert.ToInt32( data[ i ].no ) - 1 ] = true;
                        }
                    }

                    if ( ready_queue.Count > 0 )
                    {
                        ready_queue = Common.Sort_SRT( ready_queue );

                        estimate_data.Add( new ProcessData( new string[]
                        {
                            ready_queue[0].no,
                            ready_queue[0].pid,
                            ready_queue[0].priority,
                            "" + time,
                            ready_queue[0].service_time
                        } ) );
                        
                        time += Convert.ToInt32( ready_queue[ 0 ].service_time );
                        
                        ready_queue.RemoveAt( 0 );
                    }
                    else
                    {
                        time++;
                    }
                }

                List<ProcessData> time_data = GenericCopier<List<ProcessData>>.DeepCopy( estimate_data );
                time_data = Common.Sort_Default( time_data );
                data = Common.Sort_Default( data );

                for ( int idx = 0; idx < time_data.Count; idx++ )
                {
                    delay_data[ idx ] = Convert.ToInt32( time_data[ idx ].arrived_time ) - Convert.ToInt32( data[ idx ].arrived_time );
                    return_data[ idx ] = delay_data[ idx ] + Convert.ToInt32( time_data[ idx ].service_time );
                }

            }

            return estimate_data;

            //foreach (var i in estimate_data)    Console.WriteLine(i.no + "\t" + i.pid + "\t" + i.priority + "\t" + i.arrived_time + "\t" + i.service_time);
        }

        public List<int> get_wait_time()
        {
            return delay_data;
        }

        public List<int> get_return_time()
        {
            return return_data;
        }

        public double avg_wait()
        {
            //foreach ( var i in delay_data )
            //    Console.Write( i + "\t" );
            //Console.WriteLine( "\n" );

            return delay_data.Sum() / ( double )delay_data.Count;
        }

        public double avg_return()
        {
            //for ( int i = 0; i < return_data.Count; i++ )
            //{
            //    return_data[ i ] += delay_data[ i ];
            //    Console.Write( return_data[ i ] + "\t" );
            //}
            //Console.WriteLine( "\n" );

            return return_data.Sum() / ( double )return_data.Count;
        }

        private void init()
        {
            for ( int i = 0; i < data.Count; i++ )
            {
                delay_data.Add( 0 );
                return_data.Add( 0 );
                end_job_flag.Add( false );
            }
        }

        private bool end_check()
        {
            bool res = end_job_flag[ 0 ];
            foreach ( var i in end_job_flag )
            {
                if ( i )
                    res &= i;
            }
            return res;
        }
    }
}
