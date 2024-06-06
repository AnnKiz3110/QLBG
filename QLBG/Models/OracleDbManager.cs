using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using QLBG.Models;

public class OracleDbManager
{
    private string connectionString;

    public OracleDbManager()
    {
        connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
    }
    public User GetUser(string taiKhoan, string matKhau)
    {
        User user = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM USERS WHERE TaiKhoan = :TaiKhoan AND MatKhau = :MatKhau";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("TaiKhoan", taiKhoan));
                cmd.Parameters.Add(new OracleParameter("MatKhau", matKhau));

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            ID_KhachHang = Convert.ToInt32(reader["ID_KhachHang"]),
                            HoTen = reader["HoTen"].ToString(),
                            NgaySinh = Convert.ToDateTime(reader["NgaySinh"]),
                            DiaChi = reader["DiaChi"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            TaiKhoan = reader["TaiKhoan"].ToString(),
                            MatKhau = reader["MatKhau"].ToString(),
                            Role = Convert.ToInt32(reader["Role"])
                        };
                    }
                }
            }
        }
        return user;
    }

    public string RegisterUser(User user)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand("RegisterUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("p_HoTen", OracleDbType.NVarchar2)).Value = user.HoTen;
                cmd.Parameters.Add(new OracleParameter("p_NgaySinh", OracleDbType.Date)).Value = user.NgaySinh;
                cmd.Parameters.Add(new OracleParameter("p_DiaChi", OracleDbType.NVarchar2)).Value = user.DiaChi;
                cmd.Parameters.Add(new OracleParameter("p_GioiTinh", OracleDbType.NVarchar2)).Value = user.GioiTinh;
                cmd.Parameters.Add(new OracleParameter("p_Email", OracleDbType.NVarchar2)).Value = user.Email;
                cmd.Parameters.Add(new OracleParameter("p_SoDienThoai", OracleDbType.NVarchar2)).Value = user.SoDienThoai;
                cmd.Parameters.Add(new OracleParameter("p_TaiKhoan", OracleDbType.NVarchar2)).Value = user.TaiKhoan;
                cmd.Parameters.Add(new OracleParameter("p_MatKhau", OracleDbType.NVarchar2)).Value = user.MatKhau;
                cmd.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Int32)).Value = user.Role;

                var resultParam = new OracleParameter("p_Result", OracleDbType.Varchar2, 1000);
                resultParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(resultParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return resultParam.Value.ToString();
            }
        }
    }

    // Hàm lấy thông tin người dùng theo tài khoản
    public User GetUserByTaiKhoan(string taiKhoan)
    {
        User user = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM USERS WHERE TaiKhoan = :TaiKhoan";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("TaiKhoan", taiKhoan));

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            TaiKhoan = reader["TaiKhoan"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                            DiaChi = reader["DiaChi"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString()
                        };
                    }
                }
            }
        }
        return user;
    }


    

    

    // Hàm cập nhật thông tin người dùng trong cơ sở dữ liệu


    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }
        }

        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand(commandText, conn))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }

    public SanPham GetSanPham(int masanpham)
    {
        SanPham sanPham = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            using (OracleCommand cmd = new OracleCommand(@"
                SELECT sp.ID_SanPham, sp.TenSanPham, sp.ID_ThuongHieu, sp.ID_DanhMuc,
                       sp.ID_Anh, sp.ID_KichThuoc, sp.ID_Mau, sp.Mota, sp.DonViGia, sp.SoLuongTon,
                       ha.AnhChinh, ha.Anh1, ha.Anh2
                FROM SANPHAM sp
                JOIN HINHANH ha ON sp.ID_Anh = ha.ID_Anh
                WHERE sp.ID_SanPham = :ID_SanPham", conn))
            {
                cmd.Parameters.Add(new OracleParameter("ID_SanPham", masanpham));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sanPham = new SanPham
                        {
                            ID_SanPham = reader.GetInt32(reader.GetOrdinal("ID_SanPham")),
                            TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                            ID_ThuongHieu = reader.GetInt32(reader.GetOrdinal("ID_ThuongHieu")),
                            ID_DanhMuc = reader.GetInt32(reader.GetOrdinal("ID_DanhMuc")),
                            ID_Anh = reader.GetInt32(reader.GetOrdinal("ID_Anh")),
                            ID_KichThuoc = reader.GetInt32(reader.GetOrdinal("ID_KichThuoc")),
                            ID_Mau = reader.GetInt32(reader.GetOrdinal("ID_Mau")),
                            Mota = reader.GetString(reader.GetOrdinal("Mota")),
                            DonViGia = reader.GetDecimal(reader.GetOrdinal("DonViGia")),
                            SoLuongTon = reader.GetInt32(reader.GetOrdinal("SoLuongTon")),
                            AnhChinh = reader.GetString(reader.GetOrdinal("AnhChinh")),
                            Anh1 = reader.GetString(reader.GetOrdinal("Anh1")),
                            Anh2 = reader.GetString(reader.GetOrdinal("Anh2"))
                        };
                    }
                }
            }
        }
        return sanPham;
    }


    public List<SanPham> GetSanPhams()
    {
        List<SanPham> sanPhams = new List<SanPham>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM SANPHAM";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPham sanPham = new SanPham
                        {
                            ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                            TenSanPham = reader["TenSanPham"].ToString(),
                            ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                            ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                            ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                            ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                            ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                            Mota = reader["Mota"].ToString(),
                            DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                            SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                        };
                        sanPhams.Add(sanPham);
                    }
                }
            }
        }

        return sanPhams;
    }

    // Method to check if a user exists in the database

    public List<DanhMuc> GetAllDanhMucs()
    {
        List<DanhMuc> danhMucs = new List<DanhMuc>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_DanhMuc, TenDanhMuc FROM DANHMUC";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DanhMuc danhMuc = new DanhMuc
                    {
                        ID_DanhMuc = reader.GetInt32(0),
                        TenDanhMuc = reader.GetString(1)
                    };
                    danhMucs.Add(danhMuc);
                }
            }
        }

        return danhMucs;
    }

    public DanhMuc GetDanhMucById(int id)
    {
        DanhMuc danhMuc = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_DanhMuc, TenDanhMuc FROM DANHMUC WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    danhMuc = new DanhMuc
                    {
                        ID_DanhMuc = reader.GetInt32(0),
                        TenDanhMuc = reader.GetString(1)
                    };
                }
            }
        }

        return danhMuc;
    }

    public bool DanhMucExists(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM DANHMUC WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    public void InsertDanhMuc(DanhMuc danhMuc)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            OracleCommand cmd = new OracleCommand("ThemDanhMuc", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Thêm tham số cho procedure
            cmd.Parameters.Add("p_ID_DanhMuc", OracleDbType.Int32).Direction = ParameterDirection.Input;
            cmd.Parameters["p_ID_DanhMuc"].Value = danhMuc.ID_DanhMuc;

            cmd.Parameters.Add("p_TenDanhMuc", OracleDbType.NVarchar2).Direction = ParameterDirection.Input;
            cmd.Parameters["p_TenDanhMuc"].Value = danhMuc.TenDanhMuc;

            try
            {
                // Thực thi procedure
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                // Xử lý ngoại lệ
                // ex.Message chứa thông điệp lỗi từ Oracle
                throw new Exception("Lỗi khi thêm danh mục: " + ex.Message);
            }
        }
    }


    public bool UpdateDanhMuc(DanhMuc danhMuc)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE DANHMUC SET TenDanhMuc = :ten WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", danhMuc.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("ten", danhMuc.TenDanhMuc));

            int rowsUpdated = cmd.ExecuteNonQuery();
            return rowsUpdated > 0;
        }
    }

    public bool DeleteDanhMuc(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string deleteSanPhamQuery = "DELETE FROM SANPHAM WHERE ID_DanhMuc = :id";
            string deleteCTDatHangQuery = "DELETE FROM CT_DATHANG WHERE ID_SanPham IN (SELECT ID_SanPham FROM SANPHAM WHERE ID_DanhMuc = :id)";
            string deleteDanhMucQuery = "DELETE FROM DANHMUC WHERE ID_DanhMuc = :id";

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    OracleCommand cmd1 = new OracleCommand(deleteCTDatHangQuery, conn);
                    cmd1.Parameters.Add(new OracleParameter("id", id));
                    cmd1.ExecuteNonQuery();

                    OracleCommand cmd2 = new OracleCommand(deleteSanPhamQuery, conn);
                    cmd2.Parameters.Add(new OracleParameter("id", id));
                    cmd2.ExecuteNonQuery();

                    OracleCommand cmd3 = new OracleCommand(deleteDanhMucQuery, conn);
                    cmd3.Parameters.Add(new OracleParameter("id", id));
                    cmd3.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

    }

    public DataTable ShowTotalPurchase()
    {
        string query = @"
        SELECT 
            U.HoTen AS TenKhachHang, 
            SUM(CT.SoLuong * CT.DonViGia) AS TONGTIENMUAHANG
        FROM 
            USERS U
        JOIN 
            DATHANG DH ON U.ID_KhachHang = DH.ID_KhachHang
        JOIN 
            CT_DATHANG CT ON DH.ID_DatHang = CT.ID_DatHang
        WHERE 
            DH.NgayDat >= TO_DATE('2023-01-01', 'YYYY-MM-DD')
        GROUP BY 
            U.HoTen
        HAVING 
            SUM(CT.SoLuong * CT.DonViGia) > 500
        ORDER BY 
            TONGTIENMUAHANG DESC";

        return ExecuteQuery(query);
    }

    public DataTable ShowTotalSoldProductsByCategory()
    {
        string query = @"
        SELECT 
            DM.TenDanhMuc, 
            SUM(CT.SoLuong) AS TongSoLuongBan
        FROM 
            SANPHAM SP
        JOIN 
            DANHMUC DM ON SP.ID_DanhMuc = DM.ID_DanhMuc
        JOIN 
            CT_DATHANG CT ON SP.ID_SanPham = CT.ID_SanPham
        JOIN 
            DATHANG DH ON CT.ID_DatHang = DH.ID_DatHang
        WHERE 
            DH.NgayDat >= TO_DATE('2023-01-01', 'YYYY-MM-DD')
        GROUP BY 
            DM.TenDanhMuc
        HAVING 
            SUM(CT.SoLuong) > 10
        ORDER BY 
            TongSoLuongBan DESC";

        return ExecuteQuery(query);
    }



    public List<User> GetAllUsers()
    {
        List<User> users = new List<User>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_KhachHang, HoTen, NgaySinh, DiaChi, GioiTinh, Email, SoDienThoai, TaiKhoan, MatKhau, Role FROM USERS";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = new User
                    {
                        ID_KhachHang = reader.GetInt32(0),
                        HoTen = reader.GetString(1),
                        NgaySinh = reader.GetDateTime(2),
                        DiaChi = reader.GetString(3),
                        GioiTinh = reader.GetString(4),
                        Email = reader.GetString(5),
                        SoDienThoai = reader.GetString(6),
                        TaiKhoan = reader.GetString(7),
                        MatKhau = reader.GetString(8),
                        Role = reader.GetInt32(9)
                    };
                    users.Add(user);
                }
            }
        }

        return users;
    }

    public User GetUserById(int id)
    {
        User user = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_KhachHang, HoTen, NgaySinh, DiaChi, GioiTinh, Email, SoDienThoai, TaiKhoan, MatKhau, Role FROM USERS WHERE ID_KhachHang = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        ID_KhachHang = reader.GetInt32(0),
                        HoTen = reader.GetString(1),
                        NgaySinh = reader.GetDateTime(2),
                        DiaChi = reader.GetString(3),
                        GioiTinh = reader.GetString(4),
                        Email = reader.GetString(5),
                        SoDienThoai = reader.GetString(6),
                        TaiKhoan = reader.GetString(7),
                        MatKhau = reader.GetString(8),
                        Role = reader.GetInt32(9)
                    };
                }
            }
        }

        return user;
    }

    public bool UpdateUser(User user)
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string query = @"
            UPDATE USERS 
            SET HoTen = :HoTen, 
                NgaySinh = :NgaySinh, 
                DiaChi = :DiaChi, 
                GioiTinh = :GioiTinh, 
                Email = :Email, 
                SoDienThoai = :SoDienThoai, 
                Role = :Role 
            WHERE ID_KhachHang = :ID_KhachHang";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("HoTen", user.HoTen));
                    cmd.Parameters.Add(new OracleParameter("NgaySinh", user.NgaySinh.HasValue ? (object)user.NgaySinh.Value.Date : DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiaChi", user.DiaChi));
                    cmd.Parameters.Add(new OracleParameter("GioiTinh", user.GioiTinh));
                    cmd.Parameters.Add(new OracleParameter("Email", user.Email));
                    cmd.Parameters.Add(new OracleParameter("SoDienThoai", user.SoDienThoai));
                    cmd.Parameters.Add(new OracleParameter("Role", user.Role));
                    cmd.Parameters.Add(new OracleParameter("ID_KhachHang", user.ID_KhachHang));

                    int rowsUpdated = cmd.ExecuteNonQuery();
                    return rowsUpdated > 0;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine("Error updating user: " + ex.Message);
            return false;
        }
    }




    public bool DeleteUser(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "DELETE FROM USERS WHERE ID_KhachHang = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public List<SanPham> GetAllSanPhams()
    {
        List<SanPham> sanPhams = new List<SanPham>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM SANPHAM";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SanPham sanPham = new SanPham
                    {
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        TenSanPham = reader["TenSanPham"].ToString(),
                        ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                        ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                        ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                        ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                        ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                        Mota = reader["Mota"].ToString(),
                        DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                    };
                    sanPhams.Add(sanPham);
                }
            }
        }

        return sanPhams;
    }

    public SanPham GetSanPhamById(int id)
    {
        SanPham sanPham = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM SANPHAM WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    sanPham = new SanPham
                    {
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        TenSanPham = reader["TenSanPham"].ToString(),
                        ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                        ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                        ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                        ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                        ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                        Mota = reader["Mota"].ToString(),
                        DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                    };
                }
            }
        }

        return sanPham;
    }

    public void InsertSanPham(SanPham sanPham)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "INSERT INTO SANPHAM (ID_SanPham, TenSanPham, ID_ThuongHieu, ID_DanhMuc, ID_Anh, ID_KichThuoc, ID_Mau, Mota, DonViGia, SoLuongTon) VALUES (:id, :ten, :thuongHieu, :danhMuc, :anh, :kichThuoc, :mau, :mota, :gia, :soLuongTon)";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", sanPham.ID_SanPham));
            cmd.Parameters.Add(new OracleParameter("ten", sanPham.TenSanPham));
            cmd.Parameters.Add(new OracleParameter("thuongHieu", sanPham.ID_ThuongHieu));
            cmd.Parameters.Add(new OracleParameter("danhMuc", sanPham.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("anh", sanPham.ID_Anh));
            cmd.Parameters.Add(new OracleParameter("kichThuoc", sanPham.ID_KichThuoc));
            cmd.Parameters.Add(new OracleParameter("mau", sanPham.ID_Mau));
            cmd.Parameters.Add(new OracleParameter("mota", sanPham.Mota));
            cmd.Parameters.Add(new OracleParameter("gia", sanPham.DonViGia));
            cmd.Parameters.Add(new OracleParameter("soLuongTon", sanPham.SoLuongTon));

            cmd.ExecuteNonQuery();
        }
    }

    public bool UpdateSanPham(SanPham sanPham)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE SANPHAM SET TenSanPham = :ten, ID_ThuongHieu = :thuongHieu, ID_DanhMuc = :danhMuc, ID_Anh = :anh, ID_KichThuoc = :kichThuoc, ID_Mau = :mau, Mota = :mota, DonViGia = :gia, SoLuongTon = :soLuongTon WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", sanPham.ID_SanPham));
            cmd.Parameters.Add(new OracleParameter("ten", sanPham.TenSanPham));
            cmd.Parameters.Add(new OracleParameter("thuongHieu", sanPham.ID_ThuongHieu));
            cmd.Parameters.Add(new OracleParameter("danhMuc", sanPham.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("anh", sanPham.ID_Anh));
            cmd.Parameters.Add(new OracleParameter("kichThuoc", sanPham.ID_KichThuoc));
            cmd.Parameters.Add(new OracleParameter("mau", sanPham.ID_Mau));
            cmd.Parameters.Add(new OracleParameter("mota", sanPham.Mota));
            cmd.Parameters.Add(new OracleParameter("gia", sanPham.DonViGia));
            cmd.Parameters.Add(new OracleParameter("soLuongTon", sanPham.SoLuongTon));

            int rowsUpdated = cmd.ExecuteNonQuery();
            return rowsUpdated > 0;
        }
    }

    public bool DeleteSanPham(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string deleteCTDatHangQuery = "DELETE FROM CT_DATHANG WHERE ID_SanPham = :id";
            string deleteSanPhamQuery = "DELETE FROM SANPHAM WHERE ID_SanPham = :id";

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    OracleCommand cmd1 = new OracleCommand(deleteCTDatHangQuery, conn);
                    cmd1.Parameters.Add(new OracleParameter("id", id));
                    cmd1.ExecuteNonQuery();

                    OracleCommand cmd2 = new OracleCommand(deleteSanPhamQuery, conn);
                    cmd2.Parameters.Add(new OracleParameter("id", id));
                    cmd2.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
        public HinhAnh GetAnhById(int id)
        {
            HinhAnh hinhAnh = null;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM HINHANH WHERE ID_Anh = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", id));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hinhAnh = new HinhAnh
                        {
                            ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                            AnhChinh = reader["AnhChinh"].ToString(),
                            Anh1 = reader["Anh1"].ToString(),
                            Anh2 = reader["Anh2"].ToString()
                        };
                    }
                }
            }

            return hinhAnh;
        }

    public bool SanPhamExists(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM SANPHAM WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    public List<CTDatHang> GetCTDathangBySanPhamId(int id)
    {
        List<CTDatHang> cTDathangs = new List<CTDatHang>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM CT_DATHANG WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    CTDatHang cTDathang = new CTDatHang
                    {
                        ID_CTDatHang = Convert.ToInt32(reader["ID_CT_DatHang"]),
                        ID_DatHang = Convert.ToInt32(reader["ID_DatHang"]),
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        SoLuong = Convert.ToInt32(reader["SoLuong"]),
                        DonViGia = Convert.ToDecimal(reader["DonGia"])
                    };
                    cTDathangs.Add(cTDathang);
                }
            }
        }

        return cTDathangs;
    }

    public bool DeleteAllCTDathang(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "DELETE FROM CT_DATHANG WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public int AddDatHang(int khachHangId, double tongThanhTien)
    {
        int idDatHang;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO DATHANG (ID_DATHANG, ID_KhachHang, NgayDat, SoLuong)
                    VALUES (SEQ_DATHANG.NEXTVAL, :ID_KhachHang, SYSDATE, :SoLuong)
                    RETURNING ID_DATHANG INTO :ID_DatHang";
                cmd.Parameters.Add(new OracleParameter("ID_KhachHang", khachHangId));
                cmd.Parameters.Add(new OracleParameter("SoLuong", tongThanhTien));
                OracleParameter returnParam = new OracleParameter("ID_DatHang", OracleDbType.Int32, ParameterDirection.Output);
                cmd.Parameters.Add(returnParam);

                cmd.ExecuteNonQuery();

                idDatHang = ((OracleDecimal)returnParam.Value).ToInt32();
            }
        }
        return idDatHang;
    }

    public void AddChiTietDatHang(int idDatHang, int idSanPham, int soLuong, double donViGia)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO CT_DATHANG (ID_CTDatHang, ID_DatHang, ID_SanPham, SoLuong, DonViGia)
                    VALUES (SEQ_CT_DATHANG.NEXTVAL, :ID_DatHang, :ID_SanPham, :SoLuong, :DonViGia)";
                cmd.Parameters.Add(new OracleParameter("ID_DatHang", idDatHang));
                cmd.Parameters.Add(new OracleParameter("ID_SanPham", idSanPham));
                cmd.Parameters.Add(new OracleParameter("SoLuong", soLuong));
                cmd.Parameters.Add(new OracleParameter("DonViGia", donViGia));

                cmd.ExecuteNonQuery();
            }
        }
    }



    public List<GioHang> GetGioHang(string sessionId)
    {
        // Fetch the cart items from your temporary storage based on sessionId
        // For simplicity, here we are just simulating the cart retrieval
        // You need to implement actual logic to fetch data from your session-based storage or a temporary table
        return (List<GioHang>)HttpContext.Current.Session[sessionId] ?? new List<GioHang>();
    }

    public GioHang GetSanPhamInGioHang(string sessionId, int idSanPham)
    {
        var gioHang = GetGioHang(sessionId);
        return gioHang.FirstOrDefault(sp => sp.ID_SanPham == idSanPham);
    }

    public void AddSanPhamToGioHang(string sessionId, int idSanPham, int soLuong)
    {
        var gioHang = GetGioHang(sessionId);
        var sanPham = new GioHang(idSanPham)
        {
            SoLuong = soLuong
        };
        gioHang.Add(sanPham);
        HttpContext.Current.Session[sessionId] = gioHang;
    }

    public void UpdateSoLuongSanPham(string sessionId, int idSanPham, int soLuong)
    {
        var gioHang = GetGioHang(sessionId);
        var sanPham = gioHang.FirstOrDefault(sp => sp.ID_SanPham == idSanPham);
        if (sanPham != null)
        {
            sanPham.SoLuong = soLuong;
        }
        HttpContext.Current.Session[sessionId] = gioHang;
    }

    public void RemoveSanPhamFromGioHang(string sessionId, int idSanPham)
    {
        var gioHang = GetGioHang(sessionId);
        var sanPham = gioHang.FirstOrDefault(sp => sp.ID_SanPham == idSanPham);
        if (sanPham != null)
        {
            gioHang.Remove(sanPham);
        }
        HttpContext.Current.Session[sessionId] = gioHang;
    }

    public int GetTongSoLuong(string sessionId)
    {
        var gioHang = GetGioHang(sessionId);
        return gioHang.Sum(sp => sp.SoLuong);
    }

    public double GetThanhTien(string sessionId)
    {
        var gioHang = GetGioHang(sessionId);
        return gioHang.Sum(sp => sp.ThanhTien);
    }

    public void ClearGioHang(string sessionId)
    {
        HttpContext.Current.Session[sessionId] = new List<GioHang>();
    }

    public List<DatHang> GetAllDatHangs()
    {
        List<DatHang> listDatHang = new List<DatHang>();
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT a.ID_DatHang, a.ID_KhachHang, a.NgayDat, 
                                b.ID_CTDatHang, b.ID_SanPham, b.SoLuong, b.DonViGia 
                         FROM DATHANG a 
                         JOIN CT_DATHANG b ON a.ID_DatHang = b.ID_DatHang";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idDatHang = Convert.ToInt32(reader["ID_DatHang"]);
                        var datHang = listDatHang.FirstOrDefault(d => d.ID_DatHang == idDatHang);
                        if (datHang == null)
                        {
                            datHang = new DatHang
                            {
                                ID_DatHang = idDatHang,
                                ID_KhachHang = Convert.ToInt32(reader["ID_KhachHang"]),
                                NgayDat = Convert.ToDateTime(reader["NgayDat"]),
                                CTDatHangs = new List<CTDatHang>()
                            };
                            listDatHang.Add(datHang);
                        }

                        datHang.CTDatHangs.Add(new CTDatHang
                        {
                            ID_CTDatHang = Convert.ToInt32(reader["ID_CTDatHang"]),
                            ID_DatHang = idDatHang,
                            ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                            SoLuong = Convert.ToInt32(reader["SoLuong"]),
                            DonViGia = Convert.ToDecimal(reader["DonViGia"])
                        });
                    }
                }
            }
        }
        return listDatHang;
    }


    public DatHang GetDatHangById(int id)
    {
        DatHang datHang = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT a.ID_DatHang, a.ID_KhachHang, a.NgayDat, 
                                b.ID_CTDatHang, b.ID_SanPham, b.SoLuong, b.DonViGia 
                         FROM DATHANG a 
                         JOIN CT_DATHANG b ON a.ID_DatHang = b.ID_DatHang
                         WHERE a.ID_DatHang = :ID_DatHang";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("ID_DatHang", id));
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (datHang == null)
                        {
                            datHang = new DatHang
                            {
                                ID_DatHang = Convert.ToInt32(reader["ID_DatHang"]),
                                ID_KhachHang = Convert.ToInt32(reader["ID_KhachHang"]),
                                NgayDat = Convert.ToDateTime(reader["NgayDat"]),
                                CTDatHangs = new List<CTDatHang>()
                            };
                        }

                        datHang.CTDatHangs.Add(new CTDatHang
                        {
                            ID_CTDatHang = Convert.ToInt32(reader["ID_CTDatHang"]),
                            ID_DatHang = Convert.ToInt32(reader["ID_DatHang"]),
                            ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                            SoLuong = Convert.ToInt32(reader["SoLuong"]),
                            DonViGia = Convert.ToDecimal(reader["DonViGia"])
                        });
                    }
                }
            }
        }
        return datHang;
    }

    public bool DeleteDatHang(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Delete details
                    string deleteCTQuery = "DELETE FROM CT_DATHANG WHERE ID_DatHang = :ID_DatHang";
                    using (OracleCommand deleteCTCmd = new OracleCommand(deleteCTQuery, conn))
                    {
                        deleteCTCmd.Parameters.Add(new OracleParameter("ID_DatHang", id));
                        deleteCTCmd.ExecuteNonQuery();
                    }

                    // Delete order
                    string deleteQuery = "DELETE FROM DATHANG WHERE ID_DatHang = :ID_DatHang";
                    using (OracleCommand deleteCmd = new OracleCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.Add(new OracleParameter("ID_DatHang", id));
                        deleteCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }

    // Thêm đơn hàng
    public void ThemDonHang(int idKhachHang, DateTime ngayDat, int soLuong)
    {
        using (OracleConnection connection = new OracleConnection(connectionString))
        {
            connection.Open();
            using (OracleCommand command = new OracleCommand("ThemDonHang", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("p_ID_KhachHang", OracleDbType.Int32).Value = idKhachHang;
                command.Parameters.Add("p_NgayDat", OracleDbType.Date).Value = ngayDat;
                command.Parameters.Add("p_SoLuong", OracleDbType.Int32).Value = soLuong;
                command.ExecuteNonQuery();
            }
        }
    }

    // Hiển thị đơn hàng
    public void HienThiDonHang()
    {
        using (OracleConnection connection = new OracleConnection(connectionString))
        {
            connection.Open();
            using (OracleCommand command = new OracleCommand("HienThiDonHang", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }   
    }

    // Tính tổng số lượng sản phẩm theo thương hiệu
    public int TinhTongSoLuongSanPhamTheoThuongHieu(int thuong_hieu_id)
    {
        int tongSoLuong = 0;
        using (OracleConnection connection = new OracleConnection(connectionString))
        {
            connection.Open();
            using (OracleCommand command = new OracleCommand("TINH_TONG_SO_LUONG_SAN_PHAM_THEO_THUONG_HIEU", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("thuong_hieu_id", OracleDbType.Int32).Value = thuong_hieu_id;
                command.Parameters.Add("tong_so_luong", OracleDbType.Int32).Direction = System.Data.ParameterDirection.ReturnValue;
                command.ExecuteNonQuery();
                tongSoLuong = Convert.ToInt32(command.Parameters["tong_so_luong"].Value);
            }
        }
        return tongSoLuong;
    }

}


