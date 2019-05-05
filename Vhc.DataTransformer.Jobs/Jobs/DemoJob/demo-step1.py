tableName = "a_table"
log("tableName is set using Python = " + tableName)
sql("Create or replace table "+tableName+" as select current_timestamp as time")
commit()
data = query("select time from "+tableName)
for x in data[0]:
    print(x, ':', data[0][x])