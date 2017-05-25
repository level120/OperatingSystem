﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTest
{
    class FCFS
    {   //프로세스 서비스타임이 들어감
        private List<int> process_service_time;
        private List<int> process_arrived_time;
        private List<ProcessData> data;
        //프로세스 서비스타임으로 연산된 프로세스의 대기시간이 들어감
        //평균대기시간도 계산가능
        private List<int> process_waiting_time;
        private List<int> sum_of_waitingtime;
        //반환시간
        private List<int> return_time;
        private List<ProcessData> estimate_data;
        double waiting_time_sum;
        double returng_time_sum;


        public FCFS( List<ProcessData> pre_data )
        {
            process_service_time = new List<int>();
            process_arrived_time = new List<int>();
            process_waiting_time = new List<int>();
            sum_of_waitingtime = new List<int>();
            return_time = new List<int>();
            estimate_data = new List<ProcessData>();
            this.data = new List<ProcessData>();
            this.data = pre_data;

            // working();
        }

        public List<ProcessData> working()
        {
            //Common.Sort_SRT(data)
            Common.Sort_Initialize( data );

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
            //sum_of_waitingtime
            for ( int i = 0; i < data.Count; i++ )
            {
                if ( i != 0 )
                {
                    sum_of_waitingtime.Add( process_service_time[ i ] + sum_of_waitingtime[ i - 1 ] );
                }
                else
                {
                    sum_of_waitingtime.Add( process_service_time[ i ] );
                }
            }
            //process_waiting_time
            for ( int i = 0; i < data.Count; i++ )
            {
                if ( i != 0 )
                {
                    process_waiting_time.Add( process_waiting_time[ i - 1 ] + process_service_time[ i - 1 ] );
                    //process_waiting_time.Add( sum_of_waitingtime[ i - 1 ] - process_arrived_time[ i ] );
                }
                else
                {
                    process_waiting_time.Add( process_arrived_time[ i ] );
                }
            }

            for ( int i = 0; i < data.Count; i++ )
            {

                estimate_data.Add( new ProcessData( new string[] { "" + data[ i ].no, "" + data[ i ].pid, "" + data[ i ].priority, "" + process_waiting_time[i], "" + data[ i ].service_time } ) );
            }
            
            return estimate_data;

        }
        // no work


        public double avg_wait()
        {
            waiting_time_sum = process_waiting_time.Sum();
            return ( waiting_time_sum ) / ( data.Count );
        }

        public double avg_return()//어라이브 타임,서비스 타임
        {
            for ( int i = 0; i < data.Count; i++ )
            {
                return_time.Add( sum_of_waitingtime[ i ] - process_arrived_time[ i ] );
            }
            returng_time_sum = return_time.Sum();

            return ( returng_time_sum ) / ( data.Count );
        }
    }
}