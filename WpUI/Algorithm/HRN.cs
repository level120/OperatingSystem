using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTest
{
    class HRN
    {
        private List<ProcessData> data;
        private List<ProcessData> estimate_data;

        private List<int> delay_data;
        private List<int> return_data;
        private List<bool> end_job_flag;

        public HRN( List<ProcessData> pre_data )
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
                        if ( Convert.ToInt32( data[ i ].arrived_time ) <= time  && 
                            !end_job_flag[Convert.ToInt32(data[i].no) - 1] )
                        {
                            ready_queue.Add( data[ i ] );
                            // 작업 수행 확인
                            end_job_flag[ Convert.ToInt32( data[ i ].no ) - 1 ] = true;
                        }
                    }

                    if ( ready_queue.Count > 0 )
                    {
                        ready_queue = calculate( time, ready_queue );

                        estimate_data.Add( new ProcessData( new string[]
                        {
                            ready_queue[0].no,
                            ready_queue[0].pid,
                            ready_queue[0].priority,
                            "" + time,
                            ready_queue[0].service_time
                        } ) );

                        // 대기시간 계산
                        delay_data[ Convert.ToInt32( ready_queue[ 0 ].pid ) - Common.START_PID ] = ( time - Convert.ToInt32( data[ Convert.ToInt32( ready_queue[ 0 ].no ) - 1 ].arrived_time ) - return_data[ Convert.ToInt32( ready_queue[ 0 ].no ) - 1 ] );
                        // 반환시간 계산
                        time += return_data[ Convert.ToInt32( ready_queue[ 0 ].pid ) - Common.START_PID ] = Convert.ToInt32( ready_queue[ 0 ].service_time );
                        
                        ready_queue.RemoveAt( 0 );
                    }
                    else
                    {
                        time++;
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
                end_job_flag.Add( false );
            }
        }

        private bool end_check()
        {
            bool res = end_job_flag[ 0 ];
            foreach (var i in end_job_flag)
            {
                if ( i )
                    res &= i;
            }
            return res;
        }

        private List<ProcessData> calculate(int time, List<ProcessData> data )
        {
            List<ProcessData> calculate_data = new List<ProcessData>();
            List<Cal_Temp> calc = new List<Cal_Temp>();
            
            for ( int i = 0; i < data.Count; i++ )
            {
                double response = ( time - Convert.ToDouble( data[ i ].arrived_time ) ) / Convert.ToDouble( data[ i ].service_time ) + 1;

                calc.Add( new Cal_Temp( new string[] {
                    data[ i ].no,
                    data[ i ].pid,
                    data[ i ].priority,
                    data[ i ].arrived_time,
                    data[ i ].service_time },
                    response
                    ) );
            }

            calc = Common.Sort_HRN( calc );

            for ( int i = 0; i < calc.Count; i++ )
            {
                calculate_data.Add( new ProcessData( new string[]
                {
                    calc[i].no,
                    calc[i].pid,
                    calc[i].priority,
                    calc[i].arrived_time,
                    calc[i].service_time
                } ) );
            }

            return calculate_data;
        }
    }
}
