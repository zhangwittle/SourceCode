package com.wittle.sourcecode.utils;

import java.util.Calendar;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;

import com.wittle.sourcecode.common.log.CFLoggerFactory;
import com.wittle.sourcecode.common.log.ICFLogger;

/**
 * 类型转换工具类 Created by Pai on 2016/3/2.
 */
public class PropertyUtil {
	private static final ICFLogger logger = CFLoggerFactory.getLogger(PropertyUtil.class);
	static String ARRAY_SEPARATOR = "";

	public static int[] getIntArrayFromStringBySeparator(String str) {
		int[] intArray = new int[0];

		if (str == null || str.equals("")) {
			return intArray;
		}
		String[] strArray = str.split("");

		try {
			intArray = new int[strArray.length];
			for (int i = 0; i < intArray.length; i++) {
				intArray[i] = Integer.parseInt(strArray[i]);
			}
		} catch (Exception e) {
			logger.error("String to IntArray error: " + str);
		}
		return intArray;
	}

	public static String getStringFromIntArrayBySeparator(int[] intArray) {
		String str = "";

		if (intArray == null || intArray.length <= 0) {
			return str;
		}
		for (int i = 0; i < intArray.length; i++) {
			if (i == 0) {
				str += intArray[i];
			} else {
				str += ARRAY_SEPARATOR + intArray[i];
			}
		}
		return str;
	}

	public static Long[] getLongArrayFromStringBySeparator(String str) {
		Long[] intArray = new Long[0];

		if (str == null || str.equals("")) {
			return intArray;
		}
		String[] strArray = str.split(ARRAY_SEPARATOR);

		try {
			intArray = new Long[strArray.length];
			for (int i = 0; i < intArray.length; i++) {
				intArray[i] = Long.parseLong(strArray[i]);
			}
		} catch (Exception e) {
			logger.error("String to IntArray error: " + str);
		}
		return intArray;
	}

	public static String getStringFromLongArrayBySeparator(Long[] intArray) {
		String str = "";

		if (intArray == null || intArray.length <= 0) {
			return str;
		}
		for (int i = 0; i < intArray.length; i++) {
			if (i == 0) {
				str += intArray[i];
			} else {
				str += ARRAY_SEPARATOR + intArray[i];
			}
		}
		return str;
	}

	public static boolean isRepeatedInIntArray(int[] intArray) {
		for (int i = 0; i < intArray.length; i++) {
			int count = 0;
			for (int j = 0; j < intArray.length; j++) {
				// 有重复值就count+1
				if (intArray[i] == intArray[j]) {
					count++;
				}
			}
			// 由于中间又一次会跟自己本身比较所有这里要判断count>=2
			if (count >= 2) {
				return true;
			}
		}
		return false;
	}

	public static boolean containsInIntArray(int[] intArray, int value) {
		for (int i = 0; i < intArray.length; i++) {
			if (intArray[i] == value) {
				return true;
			}
		}
		return false;
	}

	/**
	 * 获取特定小时对于的时间戳
	 * 
	 * @param hour
	 * @return
	 */
	public static long getTimesOfHour(int hour) {
		Calendar cal = Calendar.getInstance();
		cal.set(Calendar.HOUR_OF_DAY, hour);
		cal.set(Calendar.SECOND, 0);
		cal.set(Calendar.MINUTE, 0);
		cal.set(Calendar.MILLISECOND, 0);
		return cal.getTimeInMillis();
	}

	/**
	 * 获取特定小时对于的时间戳
	 * 
	 * @param hour
	 * @return
	 */
	public static long getTimesOfDayAndHour(int day, int hour) {
		Calendar cal = Calendar.getInstance();
		cal.set(Calendar.DAY_OF_WEEK, day);
		cal.set(Calendar.HOUR_OF_DAY, hour);
		cal.set(Calendar.SECOND, 0);
		cal.set(Calendar.MINUTE, 0);
		cal.set(Calendar.MILLISECOND, 0);
		return cal.getTimeInMillis();
	}

	// 独立随机：固定收益，随机收益
	public static Map<Integer, String> independentRandom(int[] ids, int[] idProbabilities, int[] types, String[] numArray) {
		if (ids == null || idProbabilities == null || types == null || numArray == null) {
			logger.error("independentRandom error");
			return null;
		}
		Map<Integer, String> resultMap = new HashMap<>();
		if (ids.length == idProbabilities.length && ids.length == numArray.length) {
			// 固定收益，随机收益
			for (int j = 0; j < ids.length; j++) {
				int id = ids[j];
				int type = types[j];
				int idProbability = idProbabilities[j];
				String number = numArray[j];
				String[] nums = number.split("-");
				int minNum = Integer.parseInt(nums[0]);
				int maxNum = Integer.parseInt(nums[1]);

				int probability = (int) (1 + Math.random() * 100);
				if (probability <= idProbability) {
					Random random = new Random();
					int num = minNum + random.nextInt(maxNum - minNum + 1);
					if (resultMap.containsKey(id)) {
						String temp = resultMap.get(id);
						String[] values = temp.split(",");
						int count = Integer.valueOf(values[1]);
						count = count + num;
						resultMap.put(id, type + "," + count);
					} else {
						resultMap.put(id, type + "," + num);
					}
				}
			}
			return resultMap;
		} else {
			logger.error("ids,idProbabilities and numArray, length are not of same length");
			return null;
		}
	}

	// 圆桌随机
	public static Map<Integer, String> roundTableRandom(int[] ids, int[] idProbabilities, int[] types, String[] numArray) {
		if (ids == null || idProbabilities == null || types == null || numArray == null) {
			logger.error("roundTableRandom error");
			return null;
		}
		Map<Integer, String> resultMap = new HashMap<>();
		float denominator = 0;
		// 所有项加起来作为分母
		for (int i : idProbabilities) {
			denominator += i;
		}
		if (ids.length == idProbabilities.length && ids.length == numArray.length) {
			// 圆桌随机
			double probability = Math.random();
			double pren = 0;
			int tagId = 0;
			int flag = 0;
			for (int j = 0; j < idProbabilities.length; j++) {
				// 配置表中各个字段的概率
				pren += (idProbabilities[j] / denominator);
				if (probability <= pren) {
					tagId = ids[j];
					flag = j;
					break;
				}
			}
			String number = numArray[flag];
			int type = types[flag];
			String[] nums = number.split("-");
			int minNum = Integer.parseInt(nums[0]);
			int maxNum = Integer.parseInt(nums[1]);
			Random random = new Random();
			int num = minNum + random.nextInt(maxNum - minNum + 1);
			if (resultMap.containsKey(tagId)) {
				String temp = resultMap.get(tagId);
				String[] values = temp.split(",");
				int count = Integer.valueOf(values[1]);
				count = count + num;
				resultMap.put(tagId, type + "," + count);
			} else {
				resultMap.put(tagId, type + "," + num);
			}
			return resultMap;
		} else {
			logger.error("ids,idProbabilities and numArray, length are not of same length");
			return null;
		}

	}

	public static String[] StringArrayParse(String value) {
		value = value.trim();
		value = value.replace("(", "");
		value = value.replace(")", "");
		value = value.replace("[", "");
		value = value.replace("]", "");
		String[] strArray = value.split(",");
		return strArray;
	}

	public static int[] IntArrayParse(String value) {
		if (value == null || value.equals("[]"))
			return null;

		String[] intStrArray = StringArrayParse(value);
		int[] intArray = new int[intStrArray.length];
		for (int i = 0; i < intStrArray.length; i++) {
			intArray[i] = Integer.parseInt(intStrArray[i]);
		}
		return intArray;
	}

	public static float[] FloatArrayParse(String value) {
		if (value == null || value.equals("[]"))
			return null;

		String[] floatStrArray = StringArrayParse(value);
		float[] floatArray = new float[floatStrArray.length];
		for (int i = 0; i < floatStrArray.length; i++) {
			floatArray[i] = Float.parseFloat(floatStrArray[i]);
		}
		return floatArray;
	}

}
