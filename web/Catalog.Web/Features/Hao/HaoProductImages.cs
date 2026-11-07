namespace Catalog.Web.Features.Hao;

public static class HaoProductImages
{
    public static string ForDemoProduct(int id) => id switch
    {
        101 => "/images/products/intel_i5.png",
        102 => "/images/products/kingston_ram16.png",
        103 => "/images/products/samsung_990pro_1tb.png",
        104 => "/images/products/minh-hoa-psu.svg",
        _ => "/images/products/minh-hoa-chung.svg"
    };
}
