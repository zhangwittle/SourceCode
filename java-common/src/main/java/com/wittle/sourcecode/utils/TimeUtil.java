package com.wittle.sourcecode.utils;

import java.sql.Timestamp;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;
import java.util.concurrent.TimeUnit;

import com.wittle.sourcecode.common.log.CFLoggerFactory;
import com.wittle.sourcecode.common.log.ICFLogger;

public class TimeUtil {
	protected static ICFLogger logger = CFLoggerFactory.getLogger(TimeUtil.class);

	/**
	 * 每天的毫秒数
	 */
	public static final long DAY = TimeUnit.DAYS.toMillis(1);
	/**
	 * 每小时的毫秒数
	 */
	public static final long HOUR = TimeUnit.HOURS.toMillis(1);
	/**
	 * 每分的毫秒数
	 */
	public static final long MINUTE = TimeUnit.MINUTES.toMillis(1);
	/**
	 * 每秒的毫秒数
	 */
	public static final long SECOND = TimeUnit.SECONDS.toMillis(1);
	// 一小时的分钟数
	public static final long MINUTES_OF_HOUR = HOUR / MINUTE;

	public static final long SECONDS_OF_MINUTE = MINUTE / SECOND;
	/**
	 * 一周的天数
	 */
	public static final int DAY_OF_WEEK = 7;

	/**
	 * 时间格式yyyy/MM/dd
	 */
	public static final String DATE_FORMAT1 = "yyyy/MM/dd HH:mm:ss";
	public static final SimpleDateFormat dateFormat = new SimpleDateFormat(DATE_FORMAT1);

	/**
	 * 以0点为界， 只要跨越零点即算作一天， 比如 12-1 23:00 和12-2 01:00相差1天， 输入参数start < end 返回正数,
	 * start > end 返回负数
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static int getSoFarWentDays(Calendar start, Calendar end) {

		int sign = start.before(end) ? 1 : -1;
		if (end.before(start)) {
			Calendar tmp = end;
			end = start;
			start = tmp;
		}
		int days = end.get(Calendar.DAY_OF_YEAR) - start.get(Calendar.DAY_OF_YEAR);
		if (start.get(Calendar.YEAR) != end.get(Calendar.YEAR)) {
			Calendar cloneSt = (Calendar) start.clone();
			while (cloneSt.get(Calendar.YEAR) != end.get(Calendar.YEAR)) {
				days += cloneSt.getActualMaximum(Calendar.DAY_OF_YEAR);
				cloneSt.add(Calendar.YEAR, 1);
			}
		}

		return days * sign;
	}

	public static int getSoFarWentDays(Timestamp start, Timestamp end) {
		Calendar calendarStart = Calendar.getInstance();
		Calendar calendarEnd = Calendar.getInstance();
		calendarStart.setTime(start);
		calendarEnd.setTime(end);

		return getSoFarWentDays(calendarStart, calendarEnd);
	}

	/**
	 * 得到相差分钟数 start > end 返回负数
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static long getSoFarWentMinutes(Date start, Date end) {
		return (end.getTime() - start.getTime()) / MINUTE;
	}

	/**
	 * 得到相差秒数 start > end 返回负数
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static long getSoFarWentSeconds(Date start, Date end) {
		return (end.getTime() - start.getTime()) / SECOND;
	}

	/**
	 * 是否是同一天
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static boolean isSameDay(long start, long end) {
		Calendar st = Calendar.getInstance();
		st.setTimeInMillis(start);
		Calendar et = Calendar.getInstance();
		et.setTimeInMillis(end);

		return st.get(Calendar.YEAR) == et.get(Calendar.YEAR) && st.get(Calendar.MONTH) == et.get(Calendar.MONTH) && st.get(Calendar.DAY_OF_MONTH) == et.get(Calendar.DAY_OF_MONTH);

	}

	/**
	 * 判断start和end是否在同一个星期内(周一为一周开始)
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static boolean isSameWeek(long start, long end) {
		Calendar st = Calendar.getInstance();
		st.setTimeInMillis(start);
		Calendar et = Calendar.getInstance();
		et.setTimeInMillis(end);
		int days = Math.abs(TimeUtil.getSoFarWentDays(st, et));
		if (days < TimeUtil.DAY_OF_WEEK) {
			// 设置Monday为一周的开始
			st.setFirstDayOfWeek(Calendar.MONDAY);
			et.setFirstDayOfWeek(Calendar.MONDAY);
			// System.out.println(et.get(Calendar.WEEK_OF_YEAR));
			// System.out.println(st.get(Calendar.WEEK_OF_YEAR));
			if (st.get(Calendar.WEEK_OF_YEAR) == et.get(Calendar.WEEK_OF_YEAR)) {
				return true;
			}
		}
		return false;
	}

	/**
	 * 判断start和end是否在同一个月内
	 *
	 * @param start
	 * @param end
	 * @return
	 */
	public static boolean isSameMonth(long start, long end) {
		Calendar st = Calendar.getInstance();
		st.setTimeInMillis(start);
		Calendar et = Calendar.getInstance();
		et.setTimeInMillis(end);
		if (st.get(Calendar.YEAR) == et.get(Calendar.YEAR) && st.get(Calendar.MONTH) == et.get(Calendar.MONTH))
			return true;
		return false;
	}

	/**
	 * 根据输入的时间格式将输入的字符串转换成时间
	 *
	 * @param dateString
	 * @return
	 */
	public static Date convertString2Date(String dateString, String formatString) {
		dateFormat.applyPattern(DATE_FORMAT1);
		try {
			return dateFormat.parse(dateString);
		} catch (ParseException e) {
			e.printStackTrace();
		}
		return null;
	}

	/**
	 * 某个小时对应的时间戳
	 *
	 * @param time
	 * @param hour
	 * @return
	 */
	public static long timeOfSomeHour(Date time, int hour) {
		Calendar _calendar = Calendar.getInstance();
		_calendar.setTimeInMillis(time.getTime());
		_calendar.set(Calendar.HOUR_OF_DAY, hour);
		_calendar.set(Calendar.MINUTE, 0);
		_calendar.set(Calendar.SECOND, 0);
		_calendar.set(Calendar.MILLISECOND, 0);
		return _calendar.getTimeInMillis();
	}

	public static boolean isSameMonth(Date date1, Date date2) {
		Calendar _calendar1 = Calendar.getInstance();
		_calendar1.setTimeInMillis(date1.getTime());
		Calendar _calendar2 = Calendar.getInstance();
		_calendar2.setTimeInMillis(date2.getTime());
		if (_calendar1.get(Calendar.YEAR) != _calendar2.get(Calendar.YEAR) || _calendar1.get(Calendar.MONTH) != _calendar2.get(Calendar.MONTH))
			return false;
		return true;
	}

	public static boolean betweenTimeStamp(Timestamp nowTimeStamp, Timestamp startTimeStamp, Timestamp endTimeStamp) {
		if (nowTimeStamp.after(startTimeStamp) && nowTimeStamp.before(endTimeStamp)) {
			return true;
		} else {
			return false;
		}
	}

	// 到下一个格林尼治0点时间戳差距
	public static long nextGreenwichZeroHourTimeStamp() {
		Calendar calendar = Calendar.getInstance();
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		long greenwichOneHourTimeStamp = calendar.getTime().getTime() + calendar.getTimeZone().getRawOffset();
		long now = System.currentTimeMillis();
		return now >= greenwichOneHourTimeStamp ? greenwichOneHourTimeStamp + DAY : greenwichOneHourTimeStamp;
	}

	private static long testInterval = 15 * MINUTE;

	// 15 minutes
	public static long curTestStartTime(long currentTimeInMillis) {
		long result = (long) (Math.floor((double) currentTimeInMillis / testInterval) * testInterval);
		return result;
	}

	public static long curDayStartTime(long currentTimeInMillis) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeZone(TimeZone.getTimeZone("GMT"));
		calendar.setTimeInMillis(currentTimeInMillis);

		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);

		// logger.error(calendar.toString());

		return calendar.getTimeInMillis();
	}

	public static long curDayStartTimeDefaultZone(long currentTimeInMillis) {
		return currentTimeInMillis / DAY * DAY;
	}

	public static long curWeekStartTimeDefaultZone(long currentTimeInMillis) {
		return currentTimeInMillis / DAY_OF_WEEK * DAY_OF_WEEK;
	}

	/**
	 * temp
	 *
	 * @param currentTimeInMillis
	 * @return
	 */
	public static long curDayStartTime2(long currentTimeInMillis) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeZone(TimeZone.getDefault());
		calendar.setTimeInMillis(currentTimeInMillis);

		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);

		// logger.error(calendar.toString());

		return calendar.getTimeInMillis();
	}

	public static long curWeekStartTime(long currentTimeInMillis) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeZone(TimeZone.getTimeZone("GMT"));
		calendar.setTimeInMillis(currentTimeInMillis);

		calendar.set(Calendar.DAY_OF_WEEK, calendar.getActualMinimum(Calendar.DAY_OF_WEEK));
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);

		// logger.error(calendar.toString());

		return calendar.getTimeInMillis();
	}

	public static long nextWeekStartTime(long currentTimeInMillis) {
		return curWeekStartTime(currentTimeInMillis) + DAY * DAY_OF_WEEK;
	}

	public static long curMonthStartTime(long currentTimeInMillis) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeZone(TimeZone.getTimeZone("GMT"));
		calendar.setTimeInMillis(currentTimeInMillis);

		calendar.set(Calendar.DAY_OF_MONTH, calendar.getActualMinimum(Calendar.DAY_OF_MONTH));
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);

		// logger.error(calendar.toString());

		return calendar.getTimeInMillis();
	}

	public static long nextMonthStartTime(long currentTimeInMillis) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeZone(TimeZone.getTimeZone("GMT"));
		calendar.setTimeInMillis(currentTimeInMillis);

		int month = calendar.get(Calendar.MONTH);
		if (month == calendar.getActualMaximum(Calendar.MONTH)) {
			calendar.set(Calendar.YEAR, calendar.get(Calendar.YEAR) + 1);
			calendar.set(Calendar.MONTH, calendar.getActualMinimum(Calendar.MONTH));
		} else {
			calendar.set(Calendar.YEAR, calendar.get(Calendar.YEAR));
			calendar.set(Calendar.MONTH, calendar.get(Calendar.MONTH) + 1);
		}
		calendar.set(Calendar.DAY_OF_MONTH, calendar.getActualMinimum(Calendar.DAY_OF_MONTH));
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);

		// logger.error(calendar.toString());

		return calendar.getTimeInMillis();
	}

	public static int serverMilliseconds2ClientSeconds(long serverMilliseconds) {
		int clientSeconds = (int) Math.floor((double) serverMilliseconds / TimeUtil.SECOND);
		if (clientSeconds < 1) {
			clientSeconds = 1;
		}
		return clientSeconds;
	}

	public static Date getCurrentUtcDate() {
		// 1、取得本地时间：
		Calendar cal = Calendar.getInstance();
		// 2、取得时间偏移量：
		int zoneOffset = cal.get(java.util.Calendar.ZONE_OFFSET);
		// 3、取得夏令时差：
		int dstOffset = cal.get(java.util.Calendar.DST_OFFSET);
		// 4、从本地时间里扣除这些差量，即可以取得UTC时间：
		cal.add(java.util.Calendar.MILLISECOND, -(zoneOffset + dstOffset));
		return cal.getTime();
	}

	public static void main(String[] args) {
		// // 1、取得本地时间：
		// Calendar cal = Calendar.getInstance() ;
		// // 2、取得时间偏移量：
		// int zoneOffset = cal.get(java.util.Calendar.ZONE_OFFSET);
		// // 3、取得夏令时差：
		// int dstOffset = cal.get(java.util.Calendar.DST_OFFSET);
		// // 4、从本地时间里扣除这些差量，即可以取得UTC时间：
		// cal.add(java.util.Calendar.MILLISECOND, -(zoneOffset + dstOffset));
		// cal.getTime();
	}

}
