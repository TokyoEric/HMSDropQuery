import sqlite3
import xml.etree.ElementTree as ET

# 使用前修改xml和sqlite文件路径
# xml文件可以使用wz工具直接导出
item_dict = {
    "String.wz/Eqp.img.xml": 4,
    "String.wz/Etc.img.xml": 3,
    "String.wz/Ins.img.xml": 2,
    "String.wz/Consume.img.xml": 2,
    "String.wz/Cash.img.xml": 2,
}
mob_xml_path = "String.wz/Mob.img.xml"
sqlite_path = "./drop.db"


def get_dict_from_xml(path: str, level: int) -> dict:
    """从xml文件中生成ID-Name键值对
    :param path: 文件路径
    :param level: 真实信息层级
    :return:
    """
    queue = [ET.parse(path).getroot()]
    while level > 0:
        temp = queue.copy()
        queue = []
        while len(temp) > 0:
            f = temp.pop()
            for child in f:
                if child.attrib["name"] == "desc":
                    continue
                child.set("ID", f.attrib["name"])
                queue.append(child)
        level -= 1
    ret = {}
    for ele in queue:
        if ele.tag != "string":
            continue
        ret[ele.attrib["ID"]] = ele.attrib["value"]
    return ret


mob_string = get_dict_from_xml(mob_xml_path, 2)
item_string = {}
for path, level in item_dict.items():
    item_string.update(get_dict_from_xml(path, level))


def create_table(conn):
    """创建item_name和mob_name表"""
    c = conn.cursor()

    c.execute(
        """
        CREATE TABLE IF NOT EXISTS item_name (
            itemid TEXT PRIMARY KEY,
            name TEXT NOT NULL
        )
    """
    )

    c.execute(
        """
        CREATE TABLE IF NOT EXISTS mob_name (
            mobid TEXT PRIMARY KEY,
            name TEXT NOT NULL
        )
    """
    )

    conn.commit()


def insert(conn):
    """把item_string和mob_string插入sqlite数据库"""
    c = conn.cursor()
    for k, v in item_string.items():
        c.execute("INSERT INTO item_name (itemid,name) VALUES (?, ?)", (k, v))
        print("插入ITEM数据 %s %s" % (v, k))
    for k, v in mob_string.items():
        c.execute("INSERT INTO mob_name (mobid,name) VALUES (?, ?)", (k, v))
        print("插入MOB数据 %s %s" % (v, k))
    conn.commit()
    conn.close()


def main():
    conn = sqlite3.connect(sqlite_path)
    create_table(conn)
    insert(conn)
    print("done!")


if __name__ == "__main__":
    main()
