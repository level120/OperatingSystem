using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTest
{
    class SRT
    {
        private List<ProcessData> data;
        private List<ProcessData> estimate_data;

        private List<int> delay_data;
        private List<int> return_data;

        public SRT( List<ProcessData> pre_data )
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
                int working_ps_no = -1;

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
                        ready_queue = Common.Sort_SRT( ready_queue );

                        estimate_data.Add( new ProcessData( new string[]
                        {
                            ready_queue[0].no,
                            ready_queue[0].pid,
                            ready_queue[0].priority,
                            "" + time,
                            "" + Common.JOB
                        } ) );

                        // 대기시간 계산
                        if ( working_ps_no != Convert.ToInt32( ready_queue[ 0 ].no ) )
                            delay_data[ Convert.ToInt32( ready_queue[ 0 ].pid ) - Common.START_PID ] = ( time - Convert.ToInt32( data[ Convert.ToInt32( ready_queue[ 0 ].no ) - 1 ].arrived_time ) - return_data[ Convert.ToInt32( ready_queue[ 0 ].no ) - 1 ] );
                        // 반환시간 계산
                        return_data[ Convert.ToInt32( ready_queue[ 0 ].pid ) - Common.START_PID ] += Common.JOB;

                        ready_queue[ 0 ].service_time = "" + ( Convert.ToInt32( ready_queue[ 0 ].service_time ) - Common.JOB );

                        // 대기 시간 구하기 위한 flag
                        working_ps_no = Convert.ToInt32( ready_queue[ 0 ].no );

                        if ( Convert.ToInt32( ready_queue[ 0 ].service_time ) < 1 )
                            ready_queue.RemoveAt( 0 );
                    }
                }
            }

            return estimate_data;

            //foreach (var i in estimate_data)    Console.WriteLine(i.no + "\t" + i.pid + "\t" + i.priority + "\t" + i.arrived_time + "\t" + i.service_time);
        }

        public double avg_wait()
        {
            foreach ( var i in delay_data )
                Console.Write( i + "\t" );
            Console.WriteLine( "\n" );

            return delay_data.Sum() / ( double )delay_data.Count;
        }

        public double avg_return()
        {
            for ( int i = 0; i < return_data.Count; i++ )
            {
                return_data[ i ] += delay_data[ i ];
                Console.Write( return_data[ i ] + "\t" );
            }
            Console.WriteLine( "\n" );

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
