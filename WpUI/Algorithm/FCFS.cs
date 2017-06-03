using System;
using System.Collections.Generic;
using System.Linq;
//반환시간 = 대기시간 + 서비스시간
namespace AlgorithmTest
{
    class FCFS
    {   //프로세스 서비스타임이 들어감
        private List<int> process_service_time;
        private List<int> process_arrived_time;
        private List<ProcessData> data;
        private List<int> process_waiting_time;
        private List<int> return_time;
        private List<ProcessData> estimate_data;
        private List<int> start_time;
        double waiting_time_sum;
        double returng_time_sum;


        public FCFS( List<ProcessData> pre_data )
        {
            process_service_time = new List<int>();
            process_arrived_time = new List<int>();
            process_waiting_time = new List<int>();
            start_time = new List<int>();
            return_time = new List<int>();
            estimate_data = new List<ProcessData>();
            this.data = new List<ProcessData>();
            this.data = pre_data;
        }

        public List<ProcessData> working()
        {
            init();
            //Common.Sort_SRT(data)
            data = Common.Sort_Initialize( data );

            //process_service_time에 data.servicetime을 넣음
            for ( int i = 0; i < data.Count; i++ )
            {
                process_service_time.Add( Convert.ToInt32( data[ i ].service_time
                    ) );
            }

            //process_arrived_time에 data.arrivedtime을 넣음
            for ( int i = 0; i < data.Count; i++ )
            {
                process_arrived_time.Add( Convert.ToInt32( data[ i ].arrived_time
                    ) );
            }

            //start_time
            for ( int i = 0; i < data.Count; i++ )
            {
                if ( i == 0 )
                {
                    start_time.Add( process_arrived_time[ 0 ] );
                }
                else
                    if ( ( start_time[ i - 1 ] + process_service_time[ i - 1 ] ) >= ( process_arrived_time[ i ] ) )
                {
                    start_time.Add( start_time[ i - 1 ] + process_service_time[ i - 1 ] );
                }
                else
                {
                    start_time.Add( process_arrived_time[ i ] );
                }


            }


            /* 추가한 구역 wait time */
            #region

            for ( int i = 0; i < data.Count; i++ )
            {
                process_waiting_time[ Convert.ToInt32( data[ i ].pid ) - Common.START_PID ]
                    = start_time[ i ] - process_arrived_time[ i ];
            }

            #endregion
            /* 구역 끝 */


            //estimate_data
            for ( int i = 0; i < data.Count; i++ )
            {
                estimate_data.Add( new ProcessData( new string[] { "" + data[ i ].no, "" + data[ i ].pid, "" + data[ i ].priority, "" + start_time[ i ], "" + data[ i ].service_time } ) );
            }

            for ( int i = 0; i < process_waiting_time.Count; i++ )
            {
                return_time[ Convert.ToInt32( data[ i ].pid ) - Common.START_PID ]
                    = process_waiting_time[ Convert.ToInt32( data[ i ].pid ) - Common.START_PID ] + Convert.ToInt32( data[ i ].service_time );
            }
            returng_time_sum = return_time.Sum();

            return estimate_data;
        }

        public List<int> get_wait_time()
        {
            return process_waiting_time;
        }

        public List<int> get_return_time()
        {
            return return_time;
        }

        private void init()
        {
            for ( int i = 0; i < data.Count; i++ )
            {
                process_waiting_time.Add( 0 );
                return_time.Add( 0 );
            }
        }

        public double avg_wait()
        {
            waiting_time_sum = process_waiting_time.Sum();
            return ( waiting_time_sum ) / ( data.Count );
        }

        public double avg_return()//어라이브 타임,서비스 타임
        {
            return ( returng_time_sum ) / ( data.Count );
        }
    }
}

