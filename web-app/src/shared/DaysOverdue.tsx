import * as React from 'react';
import { StatusColour } from '../typeMappings';
import './DaysOverdue.scss';


interface Props {
    days: number;
    statusColor: StatusColour;
}

export const DaysOverdue: React.SFC<Props> = ({ days, statusColor }) => {

    return <div className='days_overdue'>
        <div className={statusColor}>
            {days}
        </div>
        <div>
            Days
        </div>
        </div>;
}

