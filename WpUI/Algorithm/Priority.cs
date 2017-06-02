using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTest
{
    class Priority
    {
        private List<ProcessData> data;
        private List<ProcessData> estimate_data;

        private List<int> delay_data;
        private List<int> return_data;

        public Priority( List<ProcessData> pre_data )
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

                int limit = data.Sum( item => Convert.ToInt32( item.arrived_time ) ) + data.Sum( item => Convert.ToInt32( item.service_time ) );

                init();

                for ( int time = 0; time < limit; time++ )
                {
                    for ( int i = 0; i < data.Count; i++ )
                    {
                        if ( Convert.ToInt32( data[ i ].arrived_time ) == time )
                        {
                            ready_queue.Add( data[ i ] );
                        }
                    }

                    if ( ready_queue.Count > 0 )
                    {
                        ready_queue = Common.Sort_Priority( ready_queue );

                        estimate_data.Add( new ProcessData( new string[]
                        {
                            ready_queue[0].no,
                            ready_queue[0].pid,
                            ready_queue[0].priority,
                            "" + time,
                            "" + Common.JOB
                        } ) );
                        
                        ready_queue[ 0 ].service_time = "" + ( Convert.ToInt32( ready_queue[ 0 ].service_time ) - Common.JOB );
                                                
                        /* 프로세스 시간 */
                        for ( int idx = 0; idx < ready_queue.Count; idx++ )
                        {
                            if ( ready_queue[ idx ].pid == estimate_data[ estimate_data.Count - 1 ].pid )
                            {
                                return_data[ Convert.ToInt32( ready_queue[ idx ].pid ) - Common.START_PID ]++;
                            }
                            else
                            {
                                delay_data[ Convert.ToInt32( ready_queue[ idx ].pid ) - Common.START_PID ]++;
                                return_data[ Convert.ToInt32( ready_queue[ idx ].pid ) - Common.START_PID ]++;
                            }
                        }

                        if ( Convert.ToInt32( ready_queue[ 0 ].service_time ) < 1 )
                            ready_queue.RemoveAt( 0 );
                    }
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
            }
        }
    }
}
