/* JavaScript getMonthlyWeekday Function:
 * Written by Ian L. of Jafty.com
 *
 * Description:
 * Gets Nth weekday for given month/year. For example, it can give you the date of the first monday in January, 2017 or it could give you the third Friday of June, 1999. Can get up to the fifth weekday of any given month, but will return FALSE if there is no fifth day in the given month/year.
 *
 *
 * Parameters:
 *    n = 1-5 for first, second, third, fourth or fifth weekday of the month
 *    d = full spelled out weekday Monday-Friday
 *    m = Full spelled out month like June
 *    y = Four digit representation of the year like 2017
 *
 * Return Values:
 * returns 1-31 for the date of the queried month/year that the nth weekday falls on.
 * returns false if there isn’t an nth weekday in the queried month/year
*/
export function getMonthlyWeekday(n, d, m, y){
    var targetDay, curDay=0, i=1, seekDay;
        if(d=="sunday") seekDay = 0;
        if(d=="monday") seekDay = 1;
        if(d=="tuesday") seekDay = 2;
        if(d=="wednesday") seekDay = 3;
        if(d=="thursday") seekDay = 4;
        if(d=="friday") seekDay = 5;
        if(d=="saturday") seekDay = 6;
    while(curDay < n && i < 31){
        targetDay = new Date(i++ + " " + m + " "+y);
        if(targetDay.getDay()==seekDay) curDay++;
    }
    if(curDay==n) {
        targetDay = targetDay.getDate();
        return targetDay;
    } else {
        return false;
    }
}//end getMonthlyWeekday JS function
