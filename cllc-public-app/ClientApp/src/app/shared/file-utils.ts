/**
 * Parses a file name, splitting on double underscores.
 *
 * Why? Many of the files in sharepoint use a naming convention that includes a prefix followed by double underscores.
 * The string before the underscores is recognized as the type/category of the file.
 * The string after the underscores is recognized as the file name.
 *
 * @example
 * ```typescript
 * const result1 = parseFileName('Notice__A_File_Name.pdf');
 * console.log(result1.prefix); // 'Notice'
 * console.log(result1.fileName); // 'A_File_Name.pdf'
 *
 * const result2 = parseFileName('B_File_Name.pdf');
 * console.log(result2.prefix); // ''
 * console.log(result2.fileName); // 'B_File_Name.pdf'
 * ```
 *
 * @export
 * @param {string} fileName
 * @return {*}  {{ prefix: string; fileName: string }}
 */
export function parseFileName(fileName: string): { prefix: string; fileName: string } {
  const regex = /^(.*?)__(.*)$/;

  const match = fileName.match(regex);

  if (match) {
    return { prefix: match[1], fileName: match[2] };
  }

  return { prefix: '', fileName };
}
